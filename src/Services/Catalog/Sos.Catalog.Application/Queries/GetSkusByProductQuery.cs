using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record GetSkusByProductQuery(Guid ProductId, bool OnlyActive = false)
    : IRequest<Result<IEnumerable<Sku>>>;

public class GetSkusByProductHandler(ISkuRepository repo)
    : IRequestHandler<GetSkusByProductQuery, Result<IEnumerable<Sku>>>
{
    public async Task<Result<IEnumerable<Sku>>> Handle(GetSkusByProductQuery q, CancellationToken ct)
    {
        var skus = q.OnlyActive
            ? await repo.GetActiveByProductAsync(q.ProductId, ct)
            : await repo.GetByProductAsync(q.ProductId, ct);

        return Result.Success(skus);
    }
}
