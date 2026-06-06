namespace Sos.Shared.Infrastructure.Services;

/// <summary>
/// Joriy HTTP so'rov konteksti — foydalanuvchi va tashkilot ma'lumotlari.
/// JWT tokendan o'qiladi, har bir so'rov uchun alohida instance (Scoped).
/// </summary>
public interface ICurrentContext
{
    /// <summary>Joriy foydalanuvchi ID (anonim so'rovlar uchun null)</summary>
    Guid? UserId { get; }

    /// <summary>Joriy tashkilot ID — multi-tenancy uchun asosiy kalit</summary>
    Guid? OrganizationId { get; }

    /// <summary>Foydalanuvchi roli (SuperAdmin, Owner, Admin, Member ...)</summary>
    string? Role { get; }

    /// <summary>Interfeys tili — default "uz"</summary>
    string Language { get; }

    /// <summary>Foydalanuvchi autentifikatsiyadan o'tganmi</summary>
    bool IsAuthenticated { get; }
}
