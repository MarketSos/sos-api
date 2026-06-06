using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using System.Reflection;

namespace Sos.Core.Infrastructure.Persistence;

public class CoreDbContext(
    DbContextOptions<CoreDbContext> options,
    IMediator       mediator,
    ICurrentContext context)
    : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{
    // Organization
    public DbSet<Organization>       Organizations => Set<Organization>();
    public DbSet<OrganizationMember> Members       => Set<OrganizationMember>();
    public DbSet<Address>            Addresses     => Set<Address>();

    // HR
    public DbSet<Employee>       Employees       => Set<Employee>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<EmployeeRank>   EmployeeRanks   => Set<EmployeeRank>();

    // Auth
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public override DbSet<User> Users { get; set; }
    public override DbSet<Role> Roles { get; set; }
    public override DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity tables → "Identity" schema
        builder.Entity<User>().ToTable("Users", "Identity");
        builder.Entity<Role>().ToTable("Roles", "Identity");
        builder.Entity<UserRole>().ToTable("UserRoles",   "Identity");
        builder.Entity<UserClaim>().ToTable("UserClaims", "Identity");
        builder.Entity<UserLogin>().ToTable("UserLogins", "Identity");
        builder.Entity<RoleClaim>().ToTable("RoleClaims", "Identity");
        builder.Entity<UserToken>().ToTable("UserTokens", "Identity");

        builder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);

        ApplyGlobalFilters(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        ApplyAuditInfo();
        var result = await base.SaveChangesAsync(ct);
        await PublishDomainEventsAsync(ct);
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
                typeof(CoreDbContext)
                    .GetMethod(nameof(SetAggRootWithOrgFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(clrType)
                    .Invoke(this, [modelBuilder]);
            }
            else if (isAggRoot || isSoftDel)
            {
                typeof(CoreDbContext)
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
            (context.OrganizationId == null || e.OrganizationId == context.OrganizationId));

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

        if (orgId.HasValue)
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.Entity is IHasOrganization and not AggregateRoot<Guid>
                         && e.State == EntityState.Added))
            {
                var hasOrg = (IHasOrganization)entry.Entity;
                if (hasOrg.OrganizationId == Guid.Empty)
                    hasOrg.OrganizationId = orgId.Value;
            }
        }
    }

    private async Task PublishDomainEventsAsync(CancellationToken ct)
    {
        var aggregates = ChangeTracker.Entries<Entity<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
        aggregates.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, ct);
    }
}
