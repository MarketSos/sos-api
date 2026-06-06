using Microsoft.AspNetCore.Http;

namespace Sos.Shared.Infrastructure.Services;

/// <summary>
/// CurrentContext ga ko'chirildi. Faqat backward-compat uchun qoldirilgan.
/// </summary>
[Obsolete("Use CurrentContext instead.")]
public sealed class CurrentUserService(IHttpContextAccessor accessor)
    : CurrentContext(accessor), ICurrentUserService { }
