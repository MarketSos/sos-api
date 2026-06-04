using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Identity.Application.Interfaces;
using Sos.Identity.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Identity.Application.Commands;

public record RegisterUserCommand : IRequest<Result<Guid>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public UserRole Role { get; init; } = UserRole.StoreAdmin;
    public Guid? StoreId { get; init; }
}

public class RegisterUserHandler(
    IUserRepository userRepo,
    IPasswordHasher<User> passwordHasher
) : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        var isExistEmail = await userRepo.ExistsAsync(cmd.Email, ct);

        if (isExistEmail)
            return Result.Failure<Guid>($"'{cmd.Email}' email allaqachon ro'yxatdan o'tgan.");

        var firstName = !string.IsNullOrWhiteSpace(cmd.FirstName) ? cmd.FirstName : cmd.Username;
        var tempUser = User.Create(Guid.NewGuid(), cmd.Email, "", firstName, cmd.LastName, cmd.Role, cmd.StoreId);
        var hash = passwordHasher.HashPassword(tempUser, cmd.Password);
        var user = User.Create(tempUser.Id, cmd.Email, hash, firstName, cmd.LastName, cmd.Role, cmd.StoreId);

        await userRepo.AddAsync(user, ct);
        return Result.Success(user.Id);
    }
}
