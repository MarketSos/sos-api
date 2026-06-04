using FluentValidation;
using Sos.Pricing.Application.Commands;

namespace Sos.Pricing.Application.Validators;

public class CreatePriceRuleValidator : AbstractValidator<CreatePriceRuleCommand>
{
    public CreatePriceRuleValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Mahsulot ID majburiy.");

        RuleFor(x => x.FixedPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Narx manfiy bo'lishi mumkin emas.");

        RuleFor(x => x.DiscountPct)
            .InclusiveBetween(0, 100).WithMessage("Chegirma 0 dan 100 gacha bo'lishi kerak.");

        RuleFor(x => x)
            .Must(x => x.FixedPrice > 0 || x.DiscountPct > 0)
            .WithName("FixedPrice")
            .WithMessage("FixedPrice yoki DiscountPct-dan biri ko'rsatilishi shart.");

        RuleFor(x => x.StartsAt)
            .NotEmpty().WithMessage("Boshlanish sanasi majburiy.");

        RuleFor(x => x.EndsAt)
            .Must((cmd, endsAt) => endsAt is null || endsAt > cmd.StartsAt)
            .WithMessage("Tugash sanasi boshlanish sanasidan katta bo'lishi kerak.");
    }
}

public class DeactivatePriceRuleValidator : AbstractValidator<DeactivatePriceRuleCommand>
{
    public DeactivatePriceRuleValidator()
    {
        RuleFor(x => x.RuleId)
            .NotEmpty().WithMessage("Qoida ID majburiy.");
    }
}
