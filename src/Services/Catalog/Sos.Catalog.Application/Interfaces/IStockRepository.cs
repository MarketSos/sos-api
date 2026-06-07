using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Application.Interfaces;

public interface IStockRepository
{
    Task<StockItem?>             GetAsync(Guid productId, Guid storeId, CancellationToken ct = default);
    Task<IEnumerable<StockItem>> GetByStoreAsync(Guid? storeId, CancellationToken ct = default);
    Task<IEnumerable<StockItem>> GetLowStockAsync(Guid storeId, CancellationToken ct = default);
    Task                         AddAsync(StockItem item, CancellationToken ct = default);
    Task                         SaveChangesAsync(CancellationToken ct = default);
}
