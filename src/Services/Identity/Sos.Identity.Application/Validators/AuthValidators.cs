using FluentValidation;
using Sos.Identity.Application.Commands;

namespace Sos.Identity.Application.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email majburiy.")
            .EmailAddress().WithMessage("Email formati noto'g'ri.")
            .MaximumLength(200).WithMessage("Email 200 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol majburiy.")
            .MinimumLength(8).WithMessage("Parol kamida 8 ta belgidan iborat bo'lishi kerak.")
            .MaximumLength(100).WithMessage("Parol 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ism majburiy.")
            .MaximumLength(100).WithMessage("Ism 100 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Familya majburiy.")
            .MaximumLength(100).WithMessage("Familya 100 ta belgidan oshmasligi kerak.");
    }
}

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email majburiy.")
            .EmailAddress().WithMessage("Email formati noto'g'ri.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol majburiy.");
    }
}

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken majburiy.");
    }
}
