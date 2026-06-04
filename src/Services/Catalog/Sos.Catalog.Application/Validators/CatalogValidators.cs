using FluentValidation;
using Sos.Catalog.Application.Commands;

namespace Sos.Catalog.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.NameUz)
            .NotEmpty().WithMessage("O'zbekcha nomi majburiy.")
            .MaximumLength(300).WithMessage("Nom 300 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Ruscha nomi majburiy.")
            .MaximumLength(300).WithMessage("Nom 300 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("Barcode majburiy.")
            .MaximumLength(50).WithMessage("Barcode 50 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Kategoriya majburiy.");
    }
}

public class CreateSkuValidator : AbstractValidator<CreateSkuCommand>
{
    public CreateSkuValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Mahsulot ID majburiy.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Seriya raqami majburiy.")
            .MaximumLength(100).WithMessage("Seriya raqami 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.MeasurementUnitId)
            .NotEmpty().WithMessage("O'lchov birligi majburiy.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Miqdor 0 dan katta bo'lishi kerak.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Tannarx manfiy bo'lishi mumkin emas.");

        RuleFor(x => x.SalePrice)
            .GreaterThan(0).WithMessage("Sotuv narxi 0 dan katta bo'lishi kerak.");
    }
}

public class CreateMeasurementUnitValidator : AbstractValidator<CreateMeasurementUnitCommand>
{
    public CreateMeasurementUnitValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kod majburiy.")
            .MaximumLength(20).WithMessage("Kod 20 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameUz)
            .NotEmpty().WithMessage("O'zbekcha nomi majburiy.")
            .MaximumLength(100).WithMessage("Nom 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Ruscha nomi majburiy.")
            .MaximumLength(100).WithMessage("Nom 100 ta belgidan oshmasligi kerak.");
    }
}
