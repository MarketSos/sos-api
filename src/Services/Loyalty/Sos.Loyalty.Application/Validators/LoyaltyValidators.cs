using FluentValidation;
using Sos.Loyalty.Application.Commands;

namespace Sos.Loyalty.Application.Validators;

public class CreateLoyaltyAccountValidator : AbstractValidator<CreateLoyaltyAccountCommand>
{
    public CreateLoyaltyAccountValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Mijoz ID majburiy.");
    }
}

public class EarnPointsValidator : AbstractValidator<EarnPointsCommand>
{
    public EarnPointsValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Mijoz ID majburiy.");

        RuleFor(x => x.Points)
            .GreaterThan(0).WithMessage("Yig'iladigan ball 0 dan katta bo'lishi kerak.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Izoh majburiy.")
            .MaximumLength(300).WithMessage("Izoh 300 ta belgidan oshmasligi kerak.");
    }
}

public class SpendPointsValidator : AbstractValidator<SpendPointsCommand>
{
    public SpendPointsValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Mijoz ID majburiy.");

        RuleFor(x => x.Points)
            .GreaterThan(0).WithMessage("Sarflanadigan ball 0 dan katta bo'lishi kerak.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Izoh majburiy.")
            .MaximumLength(300).WithMessage("Izoh 300 ta belgidan oshmasligi kerak.");
    }
}
