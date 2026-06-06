using FluentValidation;
using Sos.Core.Application.Commands;

namespace Sos.Core.Application.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.FirstName)
            .NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName)
            .NotEmpty().MaximumLength(100);
    }
}

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
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
