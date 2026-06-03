using MediatR;
using Sos.Identity.Application.Interfaces;
using Sos.Shared.Kernel.Results;

namespace Sos.Identity.Application.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;

public class RefreshTokenHandler(
    IUserRepository userRepo,
    IRefreshTokenRepository refreshTokenRepo,
    ITokenService tokenService
) : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
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

        var newAccess = tokenService.GenerateAccessToken(user);
        var newRefreshValue = tokenService.GenerateRefreshToken();
        var newRefresh = Domain.Entities.RefreshToken.Create(user.Id, newRefreshValue);
        await refreshTokenRepo.AddAsync(newRefresh, ct);

        return Result.Success(new LoginResponse(
            newAccess, newRefreshValue, newRefresh.ExpiresAt, user.Role.ToString()
        ));
    }
}
