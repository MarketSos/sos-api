using MediatR;
using Sos.Loyalty.Application.Interfaces;
using Sos.Loyalty.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Loyalty.Application.Commands;

// ── CreateAccount ─────────────────────────────────────────────────────────────
public record CreateLoyaltyAccountCommand(Guid CustomerId) : IRequest<Result<Guid>>;

public class CreateLoyaltyAccountHandler(ILoyaltyRepository repo)
    : IRequestHandler<CreateLoyaltyAccountCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateLoyaltyAccountCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetByCustomerIdAsync(cmd.CustomerId, ct);
        if (existing is not null)
            return Result.Failure<Guid>("Bu mijozning loyallik hisobi allaqachon mavjud.");

        var account = LoyaltyAccount.Create(cmd.CustomerId);
        await repo.AddAsync(account, ct);
        await repo.SaveChangesAsync(ct);
        return Result.Success(account.Id);
    }
}

// ── EarnPoints ────────────────────────────────────────────────────────────────
public record EarnPointsCommand(Guid CustomerId, decimal Points, string Description, Guid? SaleId = null)
    : IRequest<Result>;

public class EarnPointsHandler(ILoyaltyRepository repo)
    : IRequestHandler<EarnPointsCommand, Result>
{
    public async Task<Result> Handle(EarnPointsCommand cmd, CancellationToken ct)
    {
        var account = await repo.GetByCustomerIdAsync(cmd.CustomerId, ct);
        if (account is null) return Result.Failure("Loyallik hisobi topilmadi.");

        account.Earn(cmd.Points, cmd.Description, cmd.SaleId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── SpendPoints ───────────────────────────────────────────────────────────────
public record SpendPointsCommand(Guid CustomerId, decimal Points, string Description, Guid? SaleId = null)
    : IRequest<Result>;

public class SpendPointsHandler(ILoyaltyRepository repo)
    : IRequestHandler<SpendPointsCommand, Result>
{
    public async Task<Result> Handle(SpendPointsCommand cmd, CancellationToken ct)
    {
        var account = await repo.GetByCustomerIdAsync(cmd.CustomerId, ct);
        if (account is null) return Result.Failure("Loyallik hisobi topilmadi.");

        try
        {
            account.Spend(cmd.Points, cmd.Description, cmd.SaleId);
            await repo.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
