using MediatR;
using Sos.Pricing.Application.Interfaces;
using Sos.Pricing.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Pricing.Application.Queries;

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
