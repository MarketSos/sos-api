namespace Sos.Shared.Infrastructure.Services;

/// <summary>
/// ICurrentContext ga ko'chirildi. Ushbu interface faqat backward-compat uchun qoldirilgan.
/// </summary>
[Obsolete("Use ICurrentContext instead.")]
public interface ICurrentUserService : ICurrentContext { }
