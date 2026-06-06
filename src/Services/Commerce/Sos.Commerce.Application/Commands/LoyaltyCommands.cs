using MediatR;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Commerce.Application.Commands;

// ── CreateLoyaltyAccount ──────────────────────────────────────────────────────
public record CreateLoyaltyAccountCommand(Guid CustomerId) : IRequest<Result<Guid>>;

public class CreateLoyaltyAccountHandler(ILoyaltyRepository repo)
    : IRequestHandler<CreateLoyaltyAccountCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateLoyaltyAccountCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetByCustomerIdAsync(cmd.CustomerId, ct);
        if (existing is not null)
            return Result.Conflict<Guid, LoyaltyAccount>(cmd.CustomerId);

        var account = LoyaltyAccount.Create(cmd.CustomerId);
        await repo.AddAsync(account, ct);
        await repo.SaveChangesAsync(ct);
        return Result.Success(account.Id);
    }
}

// ── EarnPoints ────────────────────────────────────────────────────────────────
public record EarnPointsCommand(Guid CustomerId, decimal Points, string Description, Guid? SaleId = null)
    : IRequest<Result>;

public class EarnPointsHandler(ILoyaltyRepository repo) : IRequestHandler<EarnPointsCommand, Result>
{
    public async Task<Result> Handle(EarnPointsCommand cmd, CancellationToken ct)
    {
        var account = await repo.GetByCustomerIdAsync(cmd.CustomerId, ct);
        if (account is null) return Result.NotFound<LoyaltyAccount>(cmd.CustomerId);

        account.Earn(cmd.Points, cmd.Description, cmd.SaleId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── SpendPoints ───────────────────────────────────────────────────────────────
public record SpendPointsCommand(Guid CustomerId, decimal Points, string Description, Guid? SaleId = null)
    : IRequest<Result>;

public class SpendPointsHandler(ILoyaltyRepository repo) : IRequestHandler<SpendPointsCommand, Result>
{
    public async Task<Result> Handle(SpendPointsCommand cmd, CancellationToken ct)
    {
        var account = await repo.GetByCustomerIdAsync(cmd.CustomerId, ct);
        if (account is null) return Result.NotFound<LoyaltyAccount>(cmd.CustomerId);

        account.Spend(cmd.Points, cmd.Description, cmd.SaleId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
