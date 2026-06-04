using FluentValidation;
using Sos.CRM.Application.Commands;

namespace Sos.CRM.Application.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ism majburiy.")
            .MaximumLength(100).WithMessage("Ism 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Familya majburiy.")
            .MaximumLength(100).WithMessage("Familya 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Telefon raqami 20 ta belgidan oshmasligi kerak.")
            .Matches(@"^\+?[0-9\s\-\(\)]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Telefon raqami formati noto'g'ri.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email formati noto'g'ri.")
            .MaximumLength(200).WithMessage("Email 200 ta belgidan oshmasligi kerak.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.PhoneNumber) || !string.IsNullOrWhiteSpace(x.Email))
            .WithName("PhoneNumber")
            .WithMessage("Telefon raqami yoki email-dan biri ko'rsatilishi shart.");
    }
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Mijoz ID majburiy.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ism majburiy.")
            .MaximumLength(100).WithMessage("Ism 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Familya majburiy.")
            .MaximumLength(100).WithMessage("Familya 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email formati noto'g'ri.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
