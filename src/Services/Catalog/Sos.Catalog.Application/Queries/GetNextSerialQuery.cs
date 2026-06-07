using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record GetNextSerialQuery(Guid ProductId) : IRequest<Result<string>>;

public class GetNextSerialHandler(ISkuRepository repo)
    : IRequestHandler<GetNextSerialQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetNextSerialQuery q, CancellationToken ct)
    {
        var existing = await repo.GetByProductAsync(q.ProductId, ct);
        var count    = existing.Count() + 1;
        var serial   = $"BATCH-{DateTime.UtcNow:yyyyMM}-{count:D3}";
        return Result.Success(serial);
    }
}
