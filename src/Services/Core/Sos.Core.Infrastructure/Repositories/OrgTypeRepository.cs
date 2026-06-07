using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class OrgTypeRepository(CoreDbContext db) : IOrgTypeRepository
{
    public Task<List<OrgType>> GetAllAsync(CancellationToken ct = default)
        => db.OrgTypes.AsNoTracking().OrderBy(o => o.NameUz).ToListAsync(ct);

    public Task<OrgType?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.OrgTypes.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(OrgType orgType, CancellationToken ct = default)
    {
        await db.OrgTypes.AddAsync(orgType, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public async Task DeleteAsync(OrgType orgType, CancellationToken ct = default)
    {
        db.OrgTypes.Remove(orgType);
        await db.SaveChangesAsync(ct);
    }
}
