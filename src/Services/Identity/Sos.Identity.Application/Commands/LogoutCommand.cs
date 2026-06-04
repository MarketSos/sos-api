using MediatR;
using Sos.Identity.Application.Interfaces;
using Sos.Shared.Kernel.Results;

namespace Sos.Identity.Application.Commands;

public record LogoutCommand : IRequest<Result>
{
    public string RefreshToken { get; init; } = string.Empty;
}

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
