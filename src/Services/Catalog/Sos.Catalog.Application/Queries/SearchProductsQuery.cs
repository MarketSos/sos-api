using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record SearchProductsQuery(string? Q = null, Guid? CategoryId = null)
    : IRequest<Result<IEnumerable<Product>>>;

public class SearchProductsHandler(IProductRepository repo)
    : IRequestHandler<SearchProductsQuery, Result<IEnumerable<Product>>>
{
    public async Task<Result<IEnumerable<Product>>> Handle(SearchProductsQuery q, CancellationToken ct)
    {
        IEnumerable<Product> products;

        if (q.CategoryId.HasValue)
            products = await repo.GetByCategoryAsync(q.CategoryId.Value, ct);
        else if (!string.IsNullOrWhiteSpace(q.Q))
            products = await repo.SearchAsync(q.Q, ct);
        else
            products = await repo.SearchAsync(string.Empty, ct);

        return Result.Success(products);
    }
}
