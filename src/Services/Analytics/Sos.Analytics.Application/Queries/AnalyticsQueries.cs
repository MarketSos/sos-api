using MediatR;
using Sos.Analytics.Application.Interfaces;
using Sos.Shared.Kernel.Results;

namespace Sos.Analytics.Application.Queries;

// ── Sales Summary ─────────────────────────────────────────────────────────────
public record GetSalesSummaryQuery(
    Guid StoreId,
    DateTimeOffset From,
    DateTimeOffset To
) : IRequest<Result<SalesSummaryDto>>;

public class GetSalesSummaryHandler(ISaleSnapshotRepository repo)
    : IRequestHandler<GetSalesSummaryQuery, Result<SalesSummaryDto>>
{
    public async Task<Result<SalesSummaryDto>> Handle(GetSalesSummaryQuery q, CancellationToken ct)
    {
        var summary = await repo.GetSummaryAsync(q.StoreId, q.From, q.To, ct);
        return Result.Success(summary);
    }
}

// ── Revenue by Store ──────────────────────────────────────────────────────────
public record GetRevenueByStoreQuery(DateTimeOffset From, DateTimeOffset To)
    : IRequest<Result<List<(Guid StoreId, decimal Revenue)>>>;

public class GetRevenueByStoreHandler(ISaleSnapshotRepository repo)
    : IRequestHandler<GetRevenueByStoreQuery, Result<List<(Guid StoreId, decimal Revenue)>>>
{
    public async Task<Result<List<(Guid StoreId, decimal Revenue)>>> Handle(
        GetRevenueByStoreQuery q, CancellationToken ct)
    {
        var data = await repo.GetRevenueByStoreAsync(q.From, q.To, ct);
        return Result.Success(data);
    }
}
