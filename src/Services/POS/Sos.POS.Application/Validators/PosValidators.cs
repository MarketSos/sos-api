using FluentValidation;
using Sos.POS.Application.Commands;

namespace Sos.POS.Application.Validators;

public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Do'kon ID majburiy.");

        RuleFor(x => x.CashierId)
            .NotEmpty().WithMessage("Kassir ID majburiy.");
    }
}

public class AddSaleItemValidator : AbstractValidator<AddSaleItemCommand>
{
    public AddSaleItemValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Chek ID majburiy.");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Mahsulot ID majburiy.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Mahsulot nomi majburiy.")
            .MaximumLength(300).WithMessage("Mahsulot nomi 300 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miqdor 0 dan katta bo'lishi kerak.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Birlik narxi 0 dan katta bo'lishi kerak.");
    }
}

public class CompleteSaleValidator : AbstractValidator<CompleteSaleCommand>
{
    public CompleteSaleValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Chek ID majburiy.");

        RuleFor(x => x.PaidAmount)
            .GreaterThan(0).WithMessage("To'lov summasi 0 dan katta bo'lishi kerak.");
    }
}
