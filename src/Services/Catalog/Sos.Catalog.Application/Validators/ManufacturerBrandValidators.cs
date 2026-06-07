using FluentValidation;
using Sos.Catalog.Application.Commands;

namespace Sos.Catalog.Application.Validators;

public class CreateManufacturerValidator : AbstractValidator<CreateManufacturerCommand>
{
    public CreateManufacturerValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kod majburiy.")
            .MaximumLength(50).WithMessage("Kod 50 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameUz)
            .NotEmpty().WithMessage("O'zbekcha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Ruscha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");
    }
}

public class UpdateManufacturerValidator : AbstractValidator<UpdateManufacturerCommand>
{
    public UpdateManufacturerValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID majburiy.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kod majburiy.")
            .MaximumLength(50).WithMessage("Kod 50 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameUz)
            .NotEmpty().WithMessage("O'zbekcha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Ruscha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");
    }
}

public class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kod majburiy.")
            .MaximumLength(50).WithMessage("Kod 50 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameUz)
            .NotEmpty().WithMessage("O'zbekcha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Ruscha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");
    }
}

public class UpdateBrandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("ID majburiy.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kod majburiy.")
            .MaximumLength(50).WithMessage("Kod 50 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameUz)
            .NotEmpty().WithMessage("O'zbekcha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Ruscha nomi majburiy.")
            .MaximumLength(200).WithMessage("Nom 200 ta belgidan oshmasligi kerak.");
    }
}
