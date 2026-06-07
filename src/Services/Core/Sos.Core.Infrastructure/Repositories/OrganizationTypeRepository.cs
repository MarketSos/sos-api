using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class OrganizationTypeRepository(CoreDbContext db) : IOrganizationTypeRepository
{
    public Task<List<OrganizationType>> GetAllAsync(CancellationToken ct = default)
        => db.OrganizationTypes.AsNoTracking().OrderBy(o => o.NameUz).ToListAsync(ct);

    public Task<OrganizationType?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.OrganizationTypes.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(OrganizationType orgType, CancellationToken ct = default)
    {
        await db.OrganizationTypes.AddAsync(orgType, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public async Task DeleteAsync(OrganizationType orgType, CancellationToken ct = default)
    {
        db.OrganizationTypes.Remove(orgType);
        await db.SaveChangesAsync(ct);
    }
}
