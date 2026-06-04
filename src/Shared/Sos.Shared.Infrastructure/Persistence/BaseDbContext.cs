using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using System.Reflection;

namespace Sos.Shared.Infrastructure.Persistence;

/// <summary>
/// Базовый DbContext с поддержкой:
/// — автоматической публикации доменных событий
/// — глобального фильтра мягкого удаления (IsDeleted)
/// — автозаполнения аудит-полей (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
/// — глобального NoTracking для всех read-запросов
/// </summary>
public abstract class BaseDbContext(
    DbContextOptions options,
    IMediator mediator,
    ICurrentUserService currentUser) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        // Глобальный NoTracking — все read-запросы без отслеживания.
        // Для update/delete используйте Attach().State = EntityState.Modified явно.
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplySoftDeleteFilter(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        var result = await base.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// Глобальный фильтр — автоматически исключает мягко удалённые записи из всех запросов
    /// </summary>
    private static void ApplySoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)) continue;

            var method = typeof(BaseDbContext)
                .GetMethod(nameof(SetSoftDeleteFilter),
                    BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, [modelBuilder]);
        }
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDeletable
        => modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);

    /// <summary>
    /// Автозаполнение: CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
    /// Применяется только к AggregateRoot — у Entity<Guid> этих полей нет.
    /// </summary>
    private void ApplyAuditInfo()
    {
        var now    = DateTimeOffset.UtcNow;
        var userId = currentUser.UserId;

        foreach (var entry in ChangeTracker.Entries<AggregateRoot<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    if (userId.HasValue)
                        entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    if (userId.HasValue)
                        entry.Entity.UpdatedBy = userId;
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
