using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Analytics.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Analytics.Infrastructure.Persistence;

public class AnalyticsDbContext(
    DbContextOptions<AnalyticsDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<SaleSnapshot> SaleSnapshots => Set<SaleSnapshot>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("analytics");
        builder.ApplyConfigurationsFromAssembly(typeof(AnalyticsDbContext).Assembly);
    }
}
