using Sos.Core.Domain.Entities.Identity;

namespace Sos.Core.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, IList<string> roles);
    string GenerateRefreshToken();
}
