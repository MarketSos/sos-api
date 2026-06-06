using MediatR;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Commerce.Application.Queries;

public record GetLoyaltyAccountQuery(Guid CustomerId) : IRequest<Result<LoyaltyAccount>>;

public class GetLoyaltyAccountHandler(ILoyaltyRepository repo)
    : IRequestHandler<GetLoyaltyAccountQuery, Result<LoyaltyAccount>>
{
    public async Task<Result<LoyaltyAccount>> Handle(GetLoyaltyAccountQuery q, CancellationToken ct)
    {
        var account = await repo.GetByCustomerIdAsync(q.CustomerId, ct);
        return account is null
            ? Result.Failure<LoyaltyAccount>("Loyallik hisobi topilmadi.")
            : Result.Success(account);
    }
}
