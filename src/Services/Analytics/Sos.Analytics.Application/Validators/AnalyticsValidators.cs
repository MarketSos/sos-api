using FluentValidation;
using Sos.Analytics.Application.Commands;

namespace Sos.Analytics.Application.Validators;

public class RecordSaleValidator : AbstractValidator<RecordSaleCommand>
{
    public RecordSaleValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID majburiy.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Do'kon ID majburiy.");

        RuleFor(x => x.CashierId)
            .NotEmpty().WithMessage("Kassir ID majburiy.");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Summa manfiy bo'lishi mumkin emas.");

        RuleFor(x => x.ItemCount)
            .GreaterThan(0).WithMessage("Mahsulotlar soni 0 dan katta bo'lishi kerak.");

        RuleFor(x => x.CompletedAt)
            .NotEmpty().WithMessage("Tugatilgan vaqt majburiy.")
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .WithMessage("Tugatilgan vaqt kelajakda bo'lishi mumkin emas.");
    }
}
