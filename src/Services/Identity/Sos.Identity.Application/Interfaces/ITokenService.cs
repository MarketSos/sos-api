using Sos.Identity.Domain.Entities;

namespace Sos.Identity.Application.Interfaces;

public interface ITokenService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken();
}
