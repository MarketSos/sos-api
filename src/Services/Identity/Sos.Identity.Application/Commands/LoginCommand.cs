using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Identity.Application.Interfaces;
using Sos.Identity.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Identity.Application.Commands;

public record LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
public record UserInfo(Guid Id, string Email, string Role, string Username);
public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Role, UserInfo User);

public class LoginHandler(
    IUserRepository userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService
) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await userRepo.GetByEmailAsync(cmd.Email, ct);
        if (user is null || !user.IsActive)
            return Result.Failure<LoginResponse>("Email yoki parol noto'g'ri.");

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, cmd.Password);
        if (result == PasswordVerificationResult.Failed)
            return Result.Failure<LoginResponse>("Email yoki parol noto'g'ri.");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(user.Id, refreshTokenValue);

        await refreshTokenRepo.AddAsync(refreshToken, ct);

        return Result.Success(new LoginResponse(
            accessToken,
            refreshTokenValue,
            refreshToken.ExpiresAt,
            user.Role.ToString(),
            new UserInfo(user.Id, user.Email, user.Role.ToString(), user.FirstName)
        ));
    }
}
