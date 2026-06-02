using Sos.Inventory.Domain.Entities;

namespace Sos.Inventory.Application.Interfaces;

public interface IStockRepository
{
    Task<StockItem?> GetAsync(Guid productId, Guid storeId, CancellationToken ct = default);
    Task<IEnumerable<StockItem>> GetLowStockAsync(Guid storeId, CancellationToken ct = default);
    Task AddAsync(StockItem item, CancellationToken ct = default);
    Task UpdateAsync(StockItem item, CancellationToken ct = default);
}
