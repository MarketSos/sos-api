using FluentValidation;
using Sos.Core.Application.Commands;

namespace Sos.Core.Application.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().MaximumLength(50).Matches(@"^[a-zA-Z0-9_.-]+$")
            .WithMessage("UserName faqat harf, raqam, '_', '-', '.' belgilaridan iborat bo'lishi kerak.");
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.RoleIds)
            .NotEmpty().WithMessage("Kamida bitta rol ID si kiritilishi shart.");
    }
}

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
