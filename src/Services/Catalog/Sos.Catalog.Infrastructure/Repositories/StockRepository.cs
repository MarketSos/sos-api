using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Catalog.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Repositories;

public class StockRepository(CatalogDbContext db) : IStockRepository
{
    public Task<StockItem?> GetAsync(Guid productId, Guid storeId, CancellationToken ct = default)
        => db.StockItems.AsTracking()
             .FirstOrDefaultAsync(s => s.ProductId == productId && s.StoreId == storeId, ct);

    public async Task<IEnumerable<StockItem>> GetByStoreAsync(Guid? storeId, CancellationToken ct = default)
    {
        var query = db.StockItems.AsNoTracking().AsQueryable();
        if (storeId.HasValue)
            query = query.Where(s => s.StoreId == storeId.Value);
        return await query.OrderBy(s => s.ProductId).ToListAsync(ct);
    }

    public async Task<IEnumerable<StockItem>> GetLowStockAsync(Guid storeId, CancellationToken ct = default)
        => await db.StockItems
                   .Where(s => s.StoreId == storeId && s.Quantity <= s.MinQuantity)
                   .OrderBy(s => s.Quantity)
                   .ToListAsync(ct);

    public async Task AddAsync(StockItem item, CancellationToken ct = default)
        => await db.StockItems.AddAsync(item, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
