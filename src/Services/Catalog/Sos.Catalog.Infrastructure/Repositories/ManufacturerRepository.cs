using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Catalog.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Repositories;

public class ManufacturerRepository(CatalogDbContext db) : IManufacturerRepository
{
    public Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Manufacturers.AsTracking().FirstOrDefaultAsync(m => m.Id == id, ct);

    public Task<Manufacturer?> GetByCodeAsync(string code, CancellationToken ct = default)
        => db.Manufacturers.AsTracking().FirstOrDefaultAsync(
            m => m.Code == code.ToUpperInvariant(), ct);

    public Task<List<Manufacturer>> GetAllAsync(CancellationToken ct = default)
        => db.Manufacturers.OrderBy(m => m.NameUz).ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => db.Manufacturers.AnyAsync(m => m.Code == code.ToUpperInvariant(), ct);

    public async Task AddAsync(Manufacturer manufacturer, CancellationToken ct = default)
    {
        await db.Manufacturers.AddAsync(manufacturer, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
