using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Catalog.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Repositories;

public class ProductRepository(CatalogDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Products
             .Include(p => p.Category)
             .FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken ct = default)
        => db.Products.FirstOrDefaultAsync(p => p.Barcode == barcode, ct);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
        => await db.Products
                   .Where(p => p.CategoryId == categoryId && p.IsActive)
                   .ToListAsync(ct);

    public async Task<IEnumerable<Product>> SearchAsync(string query, CancellationToken ct = default)
        => await db.Products
                   .Where(p => p.IsActive && (
                       p.NameUz.Contains(query) ||
                       p.NameRu.Contains(query) ||
                       p.Barcode.Contains(query)))
                   .ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await db.Products.AddAsync(product, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Attach(product).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
    }
}

public class SkuRepository(CatalogDbContext db) : ISkuRepository
{
    public Task<Sku?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Skus
             .Include(s => s.Product)
             .Include(s => s.MeasurementUnit)
             .FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Sku?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default)
        => db.Skus.FirstOrDefaultAsync(s => s.SerialNumber == serialNumber, ct);

    public async Task<IEnumerable<Sku>> GetByProductAsync(Guid productId, CancellationToken ct = default)
        => await db.Skus
                   .Include(s => s.MeasurementUnit)
                   .Where(s => s.ProductId == productId)
                   .OrderByDescending(s => s.CreatedAt)
                   .ToListAsync(ct);

    public async Task<IEnumerable<Sku>> GetActiveByProductAsync(Guid productId, CancellationToken ct = default)
        => await db.Skus
                   .Include(s => s.MeasurementUnit)
                   .Where(s => s.ProductId == productId && s.Status == SkuStatus.Active)
                   .OrderBy(s => s.ExpirationDate)   // FIFO — muddati yaqinroq birinchi
                   .ToListAsync(ct);

    public async Task AddAsync(Sku sku, CancellationToken ct = default)
    {
        await db.Skus.AddAsync(sku, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Sku sku, CancellationToken ct = default)
    {
        db.Skus.Attach(sku).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
    }
}

public class MeasurementUnitRepository(CatalogDbContext db) : IMeasurementUnitRepository
{
    public Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.MeasurementUnits.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<MeasurementUnit?> GetByCodeAsync(string code, CancellationToken ct = default)
        => db.MeasurementUnits.FirstOrDefaultAsync(u => u.Code == code.ToLowerInvariant(), ct);

    public async Task<IEnumerable<MeasurementUnit>> GetAllAsync(CancellationToken ct = default)
        => await db.MeasurementUnits
                   .Where(u => u.IsActive)
                   .OrderBy(u => u.SortOrder)
                   .ToListAsync(ct);

    public async Task AddAsync(MeasurementUnit unit, CancellationToken ct = default)
    {
        await db.MeasurementUnits.AddAsync(unit, ct);
        await db.SaveChangesAsync(ct);
    }
}
