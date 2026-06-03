using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    Task<IEnumerable<Product>> SearchAsync(string query, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
}

public interface ISkuRepository
{
    Task<Sku?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Sku?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default);
    Task<IEnumerable<Sku>> GetByProductAsync(Guid productId, CancellationToken ct = default);

    /// <summary>
    /// Muddati o'tgan yoki tuganib ketgan SKU larni qaytarmaydi
    /// </summary>
    Task<IEnumerable<Sku>> GetActiveByProductAsync(Guid productId, CancellationToken ct = default);

    Task AddAsync(Sku sku, CancellationToken ct = default);
    Task UpdateAsync(Sku sku, CancellationToken ct = default);
}

public interface IMeasurementUnitRepository
{
    Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<MeasurementUnit?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IEnumerable<MeasurementUnit>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(MeasurementUnit unit, CancellationToken ct = default);
}
