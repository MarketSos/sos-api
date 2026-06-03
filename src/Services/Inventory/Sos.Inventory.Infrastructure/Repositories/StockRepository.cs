using Microsoft.EntityFrameworkCore;
using Sos.Inventory.Application.Interfaces;
using Sos.Inventory.Domain.Entities;
using Sos.Inventory.Infrastructure.Persistence;

namespace Sos.Inventory.Infrastructure.Repositories;

public class StockRepository(InventoryDbContext db) : IStockRepository
{
    public Task<StockItem?> GetAsync(Guid productId, Guid storeId, CancellationToken ct = default)
        => db.StockItems.FirstOrDefaultAsync(
            s => s.ProductId == productId && s.StoreId == storeId, ct);

    public async Task<IEnumerable<StockItem>> GetLowStockAsync(Guid storeId, CancellationToken ct = default)
        => await db.StockItems
                   .Where(s => s.StoreId == storeId && s.Quantity <= s.MinQuantity)
                   .OrderBy(s => s.Quantity)
                   .ToListAsync(ct);

    public async Task AddAsync(StockItem item, CancellationToken ct = default)
    {
        await db.StockItems.AddAsync(item, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(StockItem item, CancellationToken ct = default)
    {
        db.StockItems.Attach(item).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
    }
}
