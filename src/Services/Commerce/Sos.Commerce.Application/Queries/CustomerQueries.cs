using MediatR;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Commerce.Application.Queries;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<Customer>>;

public class GetCustomerByIdHandler(ICustomerRepository repo)
    : IRequestHandler<GetCustomerByIdQuery, Result<Customer>>
{
    public async Task<Result<Customer>> Handle(GetCustomerByIdQuery q, CancellationToken ct)
    {
        var c = await repo.GetByIdAsync(q.CustomerId, ct);
        return c is null ? Result.Failure<Customer>("Mijoz topilmadi.") : Result.Success(c);
    }
}

public record SearchCustomersQuery(string Query, int Limit = 20) : IRequest<Result<List<Customer>>>;

public class SearchCustomersHandler(ICustomerRepository repo)
    : IRequestHandler<SearchCustomersQuery, Result<List<Customer>>>
{
    public async Task<Result<List<Customer>>> Handle(SearchCustomersQuery q, CancellationToken ct)
    {
        var results = await repo.SearchAsync(q.Query, q.Limit, ct);
        return Result.Success(results);
    }
}
