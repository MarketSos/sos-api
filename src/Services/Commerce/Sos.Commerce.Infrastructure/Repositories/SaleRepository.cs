using Microsoft.EntityFrameworkCore;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Commerce.Infrastructure.Persistence;

namespace Sos.Commerce.Infrastructure.Repositories;

public class SaleRepository(CommerceDbContext db) : ISaleRepository
{
    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Sales.AsTracking()
             .Include(s => s.Items)
             .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<Sale>> GetByStoreAsync(
        Guid storeId, DateTime from, DateTime to, CancellationToken ct = default)
        => await db.Sales
                   .Include(s => s.Items)
                   .Where(s => s.StoreId == storeId
                            && s.CreatedAt >= from
                            && s.CreatedAt <= to)
                   .OrderByDescending(s => s.CreatedAt)
                   .ToListAsync(ct);

    public async Task AddAsync(Sale sale, CancellationToken ct = default)
        => await db.Sales.AddAsync(sale, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
