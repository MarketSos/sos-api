using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Catalog.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Repositories;

public class ProductRepository(CatalogDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken ct = default)
        => db.Products.FirstOrDefaultAsync(p => p.Barcode == barcode, ct);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
        => await db.Products.Where(p => p.CategoryId == categoryId && p.IsActive).ToListAsync(ct);

    public async Task<IEnumerable<Product>> SearchAsync(string query, CancellationToken ct = default)
        => await db.Products.Where(p => p.IsActive &&
            (p.Name.Contains(query) || p.Barcode.Contains(query))).ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await db.Products.AddAsync(product, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Update(product);
        await db