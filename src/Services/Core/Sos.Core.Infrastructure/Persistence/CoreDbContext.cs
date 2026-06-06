using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Core.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Core.Infrastructure.Persistence;

/// <summary>
/// Identity + Organization uchun birlashtirilgan DbContext.
/// Tenant filter o'chirilgan — Organization o'zi tenant.
/// </summary>
public class CoreDbContext(
    DbContextOptions<CoreDbContext> options,
    IMediator       mediator,
    ICurrentContext context)
    : BaseDbContext(options, mediator, context)
{
    protected override bool EnableTenantFilter => false;

    // Identity
    public DbSet<User>         Users         => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Organization
    public DbSet<Organization>       Organizations => Set<Organization>();
    public DbSet<OrganizationMember> Members       => Set<OrganizationMember>();
    public DbSet<Address>            Addresses     => Set<Address>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
    }
}
