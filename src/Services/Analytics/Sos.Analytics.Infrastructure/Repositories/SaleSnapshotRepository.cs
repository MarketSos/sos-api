using Microsoft.EntityFrameworkCore;
using Sos.Analytics.Application.Interfaces;
using Sos.Analytics.Domain.Entities;
using Sos.Analytics.Infrastructure.Persistence;

namespace Sos.Analytics.Infrastructure.Repositories;

public class SaleSnapshotRepository(AnalyticsDbContext db) : ISaleSnapshotRepository
{
    public async Task AddAsync(SaleSnapshot snapshot, CancellationToken ct = default)
        => await db.SaleSnapshots.AddAsync(snapshot, ct);

    public async Task<SalesSummaryDto> GetSummaryAsync(
        Guid storeId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default)
    {
        var sales = await db.SaleSnapshots
            .Where(s => s.StoreId == storeId && s.CompletedAt >= from && s.CompletedAt <= to)
            .ToListAsync(ct);

        var total  = sales.Sum(s => s.TotalAmount);
        var count  = sales.Count;
        var avg    = count > 0 ? total / count : 0;

        return new SalesSummaryDto(total, count, avg, from, to);
    }

    public async Task<List<(Guid StoreId, decimal Revenue)>> GetRevenueByStoreAsync(
        DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default)
    {
        return await db.SaleSnapshots
            .Where(s => s.CompletedAt >= from && s.CompletedAt <= to)
            .GroupBy(s => s.StoreId)
            .Select(g => new { StoreId = g.Key, Revenue = g.Sum(x => x.TotalAmount) })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.Select(x => (x.StoreId, x.Revenue)).ToList(), ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
