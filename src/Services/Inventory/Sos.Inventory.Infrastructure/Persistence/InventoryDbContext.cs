using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Inventory.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Inventory.Infrastructure.Persistence;

public class InventoryDbContext(
    DbContextOptions<InventoryDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("inventory");

        builder.Entity<StockItem>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => new { s.ProductId, s.StoreId }).IsUnique();
            e.Property(s => s.Location).HasMaxLength(100);
        });
    }
}
