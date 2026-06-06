using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class OrganizationRepository(CoreDbContext db) : IOrganizationRepository
{
    // Tracking bilan yuklash (o'zgartirish uchun). / С отслеживанием (для изменений).
    private IQueryable<Organization> WithIncludes()
        => db.Organizations
             .AsTracking()
             .Include(o => o.Members)
             .Include(o => o.Address);

    // Faqat o'qish uchun. / Только для чтения.
    private IQueryable<Organization> ReadOnly()
        => db.Organizations.AsNoTracking();

    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => WithIncludes().FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<Organization?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => WithIncludes().FirstOrDefaultAsync(o => o.Slug == slug, ct);

    public Task<Organization?> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
        => WithIncludes().FirstOrDefaultAsync(o => o.OwnerUserId == ownerUserId, ct);

    public Task<Organization?> GetByCodeAsync(string code, CancellationToken ct = default)
        => WithIncludes().FirstOrDefaultAsync(o => o.Code == code, ct);

    public Task<List<Organization>> GetAllAsync(CancellationToken ct = default)
        => ReadOnly().ToListAsync(ct);

    public Task<List<Organization>> GetByParentAsync(Guid parentId, CancellationToken ct = default)
        => ReadOnly().Where(o => o.ParentId == parentId).ToListAsync(ct);

    public Task<List<Organization>> GetChildsAsync(Guid parentId, CancellationToken ct = default)
        => ReadOnly().Where(o => o.ParentId == parentId).ToListAsync(ct);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
        => db.Organizations.AnyAsync(o => o.Slug == slug, ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => db.Organizations.AnyAsync(o => o.Code == code, ct);

    public async Task AddAsync(Organization organization, CancellationToken ct = default)
    {
        await db.Organizations.AddAsync(organization, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
