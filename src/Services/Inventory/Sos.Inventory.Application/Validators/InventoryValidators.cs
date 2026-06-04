using FluentValidation;
using Sos.Inventory.Application.Commands;

namespace Sos.Inventory.Application.Validators;

public class AddStockValidator : AbstractValidator<AddStockCommand>
{
    public AddStockValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Mahsulot ID majburiy.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Do'kon ID majburiy.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Miqdor 0 dan katta bo'lishi kerak.");

        RuleFor(x => x.MinQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Minimal miqdor manfiy bo'lishi mumkin emas.");
    }
}

public class DeductStockValidator : AbstractValidator<DeductStockCommand>
{
    public DeductStockValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Mahsulot ID majburiy.");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Do'kon ID majburiy.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Chiqim miqdori 0 dan katta bo'lishi kerak.");
    }
}
