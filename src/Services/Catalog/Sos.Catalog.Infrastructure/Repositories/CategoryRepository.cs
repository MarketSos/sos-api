using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Catalog.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Repositories;

public class CategoryRepository(CatalogDbContext db) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
        => await db.Categories.AsNoTracking().OrderBy(c => c.NameUz).ToListAsync(ct);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
}
