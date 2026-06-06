using MediatR;
using Microsoft.AspNetCore.Identity;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public record UserInfo(Guid Id, string UserName, string Email, string Role);
public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, string Role, UserInfo User);

// ── Login ─────────────────────────────────────────────────────────────────────
public record LoginCommand(string UserName, string Password) : IRequest<Result<LoginResponse>>;

public class LoginHandler(
    IRefreshTokenRepository refreshTokenRepo,
    UserManager<User>       userManager,
    ITokenService           tokenService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await userManager.FindByNameAsync(cmd.UserName);
        if (user is null || !user.IsActive)
            return Result.Failure<LoginResponse>("Login yoki parol noto'g'ri.");

        if (!await userManager.CheckPasswordAsync(user, cmd.Password))
            return Result.Failure<LoginResponse>("Login yoki parol noto'g'ri.");

        var roles             = await userManager.GetRolesAsync(user);
        var primaryRole       = roles.FirstOrDefault() ?? string.Empty;
        var accessToken       = tokenService.GenerateAccessToken(user, roles);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshToken      = RefreshToken.Create(user.Id, refreshTokenValue);

        await refreshTokenRepo.AddAsync(refreshToken, ct);

        return Result.Success(new LoginResponse(
            accessToken, refreshTokenValue, refreshToken.ExpiresAt,
            primaryRole,
            new UserInfo(user.Id, user.UserName!, user.Email!, primaryRole)));
    }
}

public record RegisterUserCommand(
    string   UserName,
    string   Email,
    string   Password,
    Guid[]   RoleIds,
    Guid?    StoreId = null
) : IRequest<Result<Guid>>;

public class RegisterUserHandler(
    UserManager<User>               userManager,
    RoleManager<Role> roleManager)
    : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand cmd, CancellationToken ct)
    {
        if (await userManager.FindByNameAsync(cmd.UserName) is not null)
            return Result.Conflict<Guid>("UserName", cmd.UserName);

        if (await userManager.FindByEmailAsync(cmd.Email) is not null)
            return Result.Conflict<Guid>("Email", cmd.Email);

        var user   = User.Create(Guid.NewGuid(), cmd.UserName, cmd.Email, cmd.StoreId);
        var result = await userManager.CreateAsync(user, cmd.Password);

        if (!result.Succeeded)
            return Result.Failure<Guid>(string.Join(", ", result.Errors.Select(e => e.Description)));

        foreach (var roleId in cmd.RoleIds)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role?.Name is not null)
                await userManager.AddToRoleAsync(user, role.Name);
        }

        return Result.Success(user.Id);
    }
}


public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;

public class RefreshTokenHandler(
    IUserRepository         userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    UserManager<User>       userManager,
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
            return Result.NotFound<LoginResponse, User>(token.UserId);

        await refreshTokenRepo.RevokeAsync(cmd.RefreshToken, ct);

        var roles           = await userManager.GetRolesAsync(user);
        var primaryRole     = roles.FirstOrDefault() ?? string.Empty;
        var newAccess       = tokenService.GenerateAccessToken(user, roles);
        var newRefreshValue = tokenService.GenerateRefreshToken();
        var newRefresh      = RefreshToken.Create(user.Id, newRefreshValue);
        await refreshTokenRepo.AddAsync(newRefresh, ct);

        return Result.Success(new LoginResponse(
            newAccess, newRefreshValue, newRefresh.ExpiresAt, primaryRole,
            new UserInfo(user.Id, user.UserName!, user.Email!, primaryRole)));
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
