using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Pricing.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Pricing.Infrastructure.Persistence;

public class PricingDbContext(
    DbContextOptions<PricingDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<PriceRule> PriceRules => Set<PriceRule>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(PricingDbContext).Assembly);
    }
}
