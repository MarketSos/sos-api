using Sos.Core.Domain.Entities;

namespace Sos.Core.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
