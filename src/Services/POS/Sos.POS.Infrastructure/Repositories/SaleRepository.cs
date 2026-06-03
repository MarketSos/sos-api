using Microsoft.EntityFrameworkCore;
using Sos.POS.Application.Interfaces;
using Sos.POS.Domain.Entities;
using Sos.POS.Infrastructure.Persistence;

namespace Sos.POS.Infrastructure.Repositories;

public class SaleRepository(PosDbContext db) : ISaleRepository
{
    public Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Sales
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
    {
        await db.Sales.AddAsync(sale, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Sale sale, CancellationToken ct = default)
    {
        db.Sales.Attach(sale).State = EntityState.Modified;

        // SaleItems ni ham update qilish kerak
        foreach (var item in sale.Items)
            db.Entry(item).State = db.Entry(item).State == EntityState.Detached
                ? EntityState.Added
                : EntityState.Modified;

        await db.SaveChangesAsync(ct);
    }
}
