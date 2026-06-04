using Sos.Analytics.Domain.Entities;

namespace Sos.Analytics.Application.Interfaces;

public record SalesSummaryDto(
    decimal TotalRevenue,
    int     TotalSales,
    decimal AverageOrderValue,
    DateTimeOffset From,
    DateTimeOffset To);

public interface ISaleSnapshotRepository
{
    Task AddAsync(SaleSnapshot snapshot, CancellationToken ct = default);
    Task<SalesSummaryDto> GetSummaryAsync(Guid storeId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
    Task<List<(Guid StoreId, decimal Revenue)>> GetRevenueByStoreAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
