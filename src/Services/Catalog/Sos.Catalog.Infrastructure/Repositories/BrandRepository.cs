using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Catalog.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Repositories;

public class BrandRepository(CatalogDbContext db) : IBrandRepository
{
    public Task<Brand?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Brands.AsTracking().FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<Brand?> GetByCodeAsync(string code, CancellationToken ct = default)
        => db.Brands.AsTracking().FirstOrDefaultAsync(
            b => b.Code == code.ToUpperInvariant(), ct);

    public Task<List<Brand>> GetAllAsync(CancellationToken ct = default)
        => db.Brands.OrderBy(b => b.NameUz).ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => db.Brands.AnyAsync(b => b.Code == code.ToUpperInvariant(), ct);

    public async Task AddAsync(Brand brand, CancellationToken ct = default)
    {
        await db.Brands.AddAsync(brand, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
