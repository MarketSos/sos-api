using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Services;

public class JwtTokenService(IConfiguration config) : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        var secret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var expiry = int.Parse(config["Jwt:ExpiryMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(ClaimTypes.Role,               user.Role.ToString()),
            new("first_name",                  user.FirstName),
            new("last_name",                   user.LastName),
            new("store_id",                    user.StoreId?.ToString() ?? ""),
        };

        if (user.OrganizationId != Guid.Empty)
            claims.Add(new Claim("org_id", user.OrganizationId.ToString()));

        var token = new JwtSecurityToken(
            issuer:             config["Jwt:Issuer"],
            audience:           config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
