using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.POS.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.POS.Infrastructure.Persistence;

public class PosDbContext(
    DbContextOptions<PosDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<Sale>     Sales     => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(PosDbContext).Assembly);
    }
}
