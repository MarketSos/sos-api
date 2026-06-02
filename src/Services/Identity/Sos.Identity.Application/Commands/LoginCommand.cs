using MediatR;
using Sos.Shared.Kernel.Results;

namespace Sos.Identity.Application.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
