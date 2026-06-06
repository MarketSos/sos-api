using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using System.Reflection;

namespace Sos.Shared.Infrastructure.Persistence;

public abstract class BaseDbContext(
    DbContextOptions options,
    IMediator mediator,
    ICurrentContext context) : DbContext(options)
{
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
            var clrType   = entityType.ClrType;
            var isAggRoot = typeof(AggregateRoot<Guid>).IsAssignableFrom(clrType);
            var hasOrg    = typeof(IHasOrganization).IsAssignableFrom(clrType);
            var isSoftDel = typeof(ISoftDeletable).IsAssignableFrom(clrType);

            if (isAggRoot && hasOrg)
            {
                typeof(BaseDbContext)
                    .GetMethod(nameof(SetAggRootWithOrgFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(clrType)
                    .Invoke(this, [modelBuilder]);
            }
            else if (isAggRoot)
            {
                typeof(BaseDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(clrType)
                    .Invoke(null, [modelBuilder]);
            }
            else if (isSoftDel && hasOrg)
            {
                typeof(BaseDbContext)
                    .GetMethod(nameof(SetSoftDeleteWithOrgFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(clrType)
                    .Invoke(this, [modelBuilder]);
            }
            else if (isSoftDel)
            {
                typeof(BaseDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(clrType)
                    .Invoke(null, [modelBuilder]);
            }
        }
    }

    private void SetAggRootWithOrgFilter<T>(ModelBuilder modelBuilder)
        where T : AggregateRoot<Guid>, IHasOrganization
        => modelBuilder.Entity<T>().HasQueryFilter(e =>
            !e.IsDeleted &&
            (!EnableTenantFilter || context.OrganizationId == null || e.OrganizationId == context.OrganizationId));

    private void SetSoftDeleteWithOrgFilter<T>(ModelBuilder modelBuilder)
        where T : class, ISoftDeletable, IHasOrganization
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
                    if (orgId.HasValue && entry.Entity is IHasOrganization aggOrg && aggOrg.OrganizationId == Guid.Empty)
                        aggOrg.OrganizationId = orgId.Value;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    if (userId.HasValue) entry.Entity.UpdatedBy = userId;
                    break;
            }
        }

        // IHasOrganization entities outside AggregateRoot (LocalizableEntity hierarchy)
        if (orgId.HasValue)
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.Entity is IHasOrganization && e.Entity is not AggregateRoot<Guid> && e.State == EntityState.Added))
            {
                var hasOrg = (IHasOrganization)entry.Entity;
                if (hasOrg.OrganizationId == Guid.Empty)
                    hasOrg.OrganizationId = orgId.Value;
            }
        }
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregates = ChangeTracker.Entries<Entity<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
        aggregates.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, cancellationToken);
    }
}
