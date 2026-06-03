using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Identity.Application.Interfaces;
using Sos.Identity.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Identity.Application.Commands;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserRole Role,
    Guid? StoreId = null
) : IRequest<Result<Guid>>;

public class RegisterUserHandler(
    IUserRepository userRepo,
    IPasswordHasher<User> passwordHasher
) : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        if (await userRepo.ExistsAsync(cmd.Email, ct))
            return Result.Failure<Guid>($"'{cmd.Email}' email allaqachon ro'yxatdan o'tgan.");

        var tempUser = User.Create(Guid.NewGuid(), cmd.Email, "", cmd.FirstName, cmd.LastName, cmd.Role, cmd.StoreId);
        var hash = passwordHasher.HashPassword(tempUser, cmd.Password);
        var user = User.Create(tempUser.Id, cmd.Email, hash, cmd.FirstName, cmd.LastName, cmd.Role, cmd.StoreId);

        await userRepo.AddAsync(user, ct);
        return Result.Success(user.Id);
    }
}
