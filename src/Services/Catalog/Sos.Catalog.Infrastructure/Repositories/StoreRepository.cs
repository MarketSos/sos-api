using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Catalog.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Repositories;

public class StoreRepository(CatalogDbContext db) : IStoreRepository
{
    public Task<Store?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Stores.AsTracking().FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);

    public Task<Store?> GetByCodeAsync(Guid organizationId, string code, CancellationToken ct = default)
        => db.Stores.AsTracking().FirstOrDefaultAsync(
            s => s.OrganizationId == organizationId &&
                 s.Code           == code.ToUpperInvariant() &&
                 !s.IsDeleted, ct);

    public Task<List<Store>> GetByOrganizationAsync(Guid organizationId, CancellationToken ct = default)
        => db.Stores
             .Where(s => s.OrganizationId == organizationId && !s.IsDeleted)
             .OrderBy(s => s.Name)
             .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(Guid organizationId, string code, CancellationToken ct = default)
        => db.Stores.AnyAsync(
            s => s.OrganizationId == organizationId &&
                 s.Code           == code.ToUpperInvariant() &&
                 !s.IsDeleted, ct);

    public async Task AddAsync(Store store, CancellationToken ct = default)
    {
        await db.Stores.AddAsync(store, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
