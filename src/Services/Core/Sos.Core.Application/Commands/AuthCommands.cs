using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public record UserInfo(Guid Id, string Email, string Role, string Username);
public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Role, UserInfo User);

// ── Login ─────────────────────────────────────────────────────────────────────
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;

public class LoginHandler(
    IUserRepository        userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    IPasswordHasher<User>  passwordHasher,
    ITokenService          tokenService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await userRepo.GetByEmailAsync(cmd.Email, ct);
        if (user is null || !user.IsActive)
            return Result.Failure<LoginResponse>("Email yoki parol noto'g'ri.");

        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, cmd.Password)
            == PasswordVerificationResult.Failed)
            return Result.Failure<LoginResponse>("Email yoki parol noto'g'ri.");

        var accessToken       = tokenService.GenerateAccessToken(user);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshToken      = RefreshToken.Create(user.Id, refreshTokenValue);

        await refreshTokenRepo.AddAsync(refreshToken, ct);

        return Result.Success(new LoginResponse(
            accessToken, refreshTokenValue, refreshToken.ExpiresAt,
            user.Role.ToString(),
            new UserInfo(user.Id, user.Email, user.Role.ToString(), user.FirstName)));
    }
}

// ── Register ──────────────────────────────────────────────────────────────────
public record RegisterUserCommand(
    string   Email,
    string   Password,
    string   FirstName,
    string   LastName,
    UserRole Role    = UserRole.StoreAdmin,
    Guid?    StoreId = null
) : IRequest<Result<Guid>>;

public class RegisterUserHandler(IUserRepository userRepo, IPasswordHasher<User> passwordHasher)
    : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        if (await userRepo.ExistsAsync(cmd.Email, ct))
            return Result.Failure<Guid>($"'{cmd.Email}' email allaqachon ro'yxatdan o'tgan.");

        var tempUser = User.Create(Guid.NewGuid(), cmd.Email, "", cmd.FirstName, cmd.LastName, cmd.Role, cmd.StoreId);
        var hash     = passwordHasher.HashPassword(tempUser, cmd.Password);
        var user     = User.Create(tempUser.Id, cmd.Email, hash, cmd.FirstName, cmd.LastName, cmd.Role, cmd.StoreId);

        await userRepo.AddAsync(user, ct);
        return Result.Success(user.Id);
    }
}

// ── RefreshToken ──────────────────────────────────────────────────────────────
public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;

public class RefreshTokenHandler(
    IUserRepository         userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    ITokenService           tokenService)
    : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var token = await refreshTokenRepo.GetByTokenAsync(cmd.RefreshToken, ct);
        if (token is null || !token.IsActive)
            return Result.Failure<LoginResponse>("Refresh token yaroqsiz yoki muddati tugagan.");

        var user = await userRepo.GetByIdAsync(token.UserId, ct);
        if (user is null || !user.IsActive)
            return Result.Failure<LoginResponse>("Foydalanuvchi topilmadi.");

        await refreshTokenRepo.RevokeAsync(cmd.RefreshToken, ct);

        var newAccess        = tokenService.GenerateAccessToken(user);
        var newRefreshValue  = tokenService.GenerateRefreshToken();
        var newRefresh       = RefreshToken.Create(user.Id, newRefreshValue);
        await refreshTokenRepo.AddAsync(newRefresh, ct);

        return Result.Success(new LoginResponse(
            newAccess, newRefreshValue, newRefresh.ExpiresAt, user.Role.ToString(),
            new UserInfo(user.Id, user.Email, user.Role.ToString(), user.FirstName)));
    }
}

// ── Logout ────────────────────────────────────────────────────────────────────
public record LogoutCommand(string RefreshToken) : IRequest<Result>;

public class LogoutHandler(IRefreshTokenRepository refreshTokenRepo)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand cmd, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(cmd.RefreshToken))
            await refreshTokenRepo.RevokeAsync(cmd.RefreshToken, ct);
        return Result.Success();
    }
}
