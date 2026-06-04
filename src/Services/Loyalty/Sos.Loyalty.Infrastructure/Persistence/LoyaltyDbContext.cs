using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Loyalty.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Loyalty.Infrastructure.Persistence;

public class LoyaltyDbContext(
    DbContextOptions<LoyaltyDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<LoyaltyAccount>     Accounts     => Set<LoyaltyAccount>();
    public DbSet<LoyaltyTransaction> Transactions => Set<LoyaltyTransaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("loyalty");
        builder.ApplyConfigurationsFromAssembly(typeof(LoyaltyDbContext).Assembly);
    }
}
