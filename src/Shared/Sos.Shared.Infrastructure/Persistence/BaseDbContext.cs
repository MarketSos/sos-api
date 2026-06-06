using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using System.Reflection;

namespace Sos.Shared.Infrastructure.Persistence;

/// <summary>
/// Barcha mikroservislar uchun asosiy DbContext. / Базовый DbContext для всех микросервисов.
/// Qo'llab-quvvatlaydi / Поддерживает:
///   — avtomatik domain event publish / автоматическая публикация доменных событий
///   — global soft-delete filter (IsDeleted) / глобальный фильтр мягкого удаления
///   — global tenant filter (OrganizationId) — override qilib o'chirish mumkin / можно отключить через override
///   — audit maydonlarini avtomatik to'ldirish / автоматическое заполнение аудит-полей
/// </summary>
public abstract class BaseDbContext(
    DbContextOptions options,
    IMediator mediator,
    ICurrentContext context) : DbContext(options)
{
    /// <summary>
    /// Tizimiy DbContextlarda (masalan Organization) false qaytaring. 
    /// Верните false для системных DbContext (например Organization).
    /// </summary>
    protected virtual bool EnableTenantFilter => true;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyGlobalFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        var result = await base.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync(cancellationToken);
        return result;
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (typeof(AggregateRoot<Guid>).IsAssignableFrom(clrType))
            {
                typeof(BaseDbContext)
                    .GetMethod(nameof(SetAggregateRootFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(clrType)
                    .Invoke(this, [modelBuilder]);
            }
            else if (typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                typeof(BaseDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(clrType)
                    .Invoke(null, [modelBuilder]);
            }
        }
    }

    private void SetAggregateRootFilter<T>(ModelBuilder modelBuilder) where T : AggregateRoot<Guid>
        => modelBuilder.Entity<T>().HasQueryFilter(e =>
            !e.IsDeleted &&
            (!EnableTenantFilter || context.OrganizationId == null || e.OrganizationId == context.OrganizationId));

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDeletable
        => modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);

    private void ApplyAuditInfo()
    {
        var now    = DateTimeOffset.UtcNow;
        var userId = context.UserId;
        var orgId  = context.OrganizationId;

        foreach (var entry in ChangeTracker.Entries<AggregateRoot<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    if (userId.HasValue) entry.Entity.CreatedBy = userId;
                    if (orgId.HasValue && entry.Entity.OrganizationId == Guid.Empty)
                        entry.Entity.OrganizationId = orgId.Value;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    if (userId.HasValue) entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
        aggregates.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, cancellationToken);
    }
}
