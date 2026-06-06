using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record GetActiveRulesQuery(Guid ProductId, Guid? StoreId = null)
    : IRequest<Result<List<PriceRule>>>;

public class GetActiveRulesHandler(IPriceRuleRepository repo)
    : IRequestHandler<GetActiveRulesQuery, Result<List<PriceRule>>>
{
    public async Task<Result<List<PriceRule>>> Handle(GetActiveRulesQuery q, CancellationToken ct)
    {
        var rules = await repo.GetActiveRulesForProductAsync(q.ProductId, q.StoreId, ct);
        return Result.Success(rules);
    }
}
