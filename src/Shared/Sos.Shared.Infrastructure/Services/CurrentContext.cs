using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Sos.Shared.Infrastructure.Services;

/// <summary>
/// JWT ClaimsPrincipaldan kontekst ma'lumotlarini o'qiydi.
/// Scoped — har bir HTTP so'rov uchun bitta instance.
/// </summary>
public class CurrentContext(IHttpContextAccessor accessor) : ICurrentContext
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? Principal?.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public Guid? OrganizationId
    {
        get
        {
            var value = Principal?.FindFirstValue("org_id");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Role => Principal?.FindFirstValue(ClaimTypes.Role);

    public string Language => Principal?.FindFirstValue("lang") ?? "uz";

    public bool IsAuthenticated
        => accessor.HttpContext?.User.Identity?.IsAuthenticated is true;
}
