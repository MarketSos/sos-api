using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Commands;

// ── CreatePriceRule ───────────────────────────────────────────────────────────
public record CreatePriceRuleCommand(
    Guid    ProductId,
    decimal FixedPrice,
    decimal DiscountPct,
    DateTimeOffset  StartsAt,
    DateTimeOffset? EndsAt,
    Guid?   StoreId = null
) : IRequest<Result<Guid>>;

public class CreatePriceRuleHandler(IPriceRuleRepository repo)
    : IRequestHandler<CreatePriceRuleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePriceRuleCommand cmd, CancellationToken ct)
    {
        if (cmd.FixedPrice < 0)
            return Result.Failure<Guid>("Narx manfiy bo'lishi mumkin emas.");
        if (cmd.DiscountPct is < 0 or > 100)
            return Result.Failure<Guid>("Chegirma 0 dan 100 gacha bo'lishi kerak.");
        if (cmd.FixedPrice == 0 && cmd.DiscountPct == 0)
            return Result.Failure<Guid>("FixedPrice yoki DiscountPct ni ko'rsating.");
        if (cmd.EndsAt.HasValue && cmd.EndsAt <= cmd.StartsAt)
            return Result.Failure<Guid>("EndsAt StartsAt dan katta bo'lishi kerak.");

        var rule = PriceRule.Create(
            cmd.ProductId, cmd.FixedPrice, cmd.DiscountPct,
            cmd.StartsAt, cmd.EndsAt, cmd.StoreId);

        await repo.AddAsync(rule, ct);
        await repo.SaveChangesAsync(ct);
        return Result.Success(rule.Id);
    }
}

// ── DeactivatePriceRule ───────────────────────────────────────────────────────
public record DeactivatePriceRuleCommand(Guid RuleId) : IRequest<Result>;

public class DeactivatePriceRuleHandler(IPriceRuleRepository repo)
    : IRequestHandler<DeactivatePriceRuleCommand, Result>
{
    public async Task<Result> Handle(DeactivatePriceRuleCommand cmd, CancellationToken ct)
    {
        var rule = await repo.GetByIdAsync(cmd.RuleId, ct);
        if (rule is null) return Result.Failure("Narx qoidasi topilmadi.");

        rule.Deactivate();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
