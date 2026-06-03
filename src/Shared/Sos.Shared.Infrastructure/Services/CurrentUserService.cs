using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Sos.Shared.Infrastructure.Services;

/// <summary>
/// Реализация — читает ID пользователя из JWT ClaimsPrincipal.
/// Регистрируется как Scoped (один экземпляр на HTTP-запрос).
/// </summary>
public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? accessor.HttpContext?.User.FindFirstValue("sub");
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated
        => accessor.HttpContext?.User.Identity?.IsAuthenticated is true;
}
