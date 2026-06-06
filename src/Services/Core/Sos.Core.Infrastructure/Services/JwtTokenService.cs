using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities.Identity;
using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Authorization;

namespace Sos.Core.Infrastructure.Services;

public class JwtTokenService(IConfiguration config) : ITokenService
{
    public string GenerateAccessToken(User user, IList<string> roles)
    {
        var secret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var expiry = int.Parse(config["Jwt:ExpiryMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new("store_id",                    user.StoreId?.ToString() ?? ""),
        };

        if (user.OrganizationId != Guid.Empty)
            claims.Add(new Claim("org_id", user.OrganizationId.ToString()));

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var permissions = roles.FirstOrDefault() switch
        {
            nameof(UserRoles.SuperAdmin) => RolePermissions.SuperAdmin,
            nameof(UserRoles.StoreAdmin) => RolePermissions.StoreAdmin,
            nameof(UserRoles.Cashier)    => RolePermissions.Cashier,
            _                           => (IReadOnlyList<string>)Array.Empty<string>()
        };

        foreach (var p in permissions)
            claims.Add(new Claim("permission", p));

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
