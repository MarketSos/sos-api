using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

/// <summary>
/// Refresh-token — ishlatilganda yangi bilan almashtiriladi.
/// Refresh-токен с ротацией.
/// </summary>
public class RefreshToken : Entity<Guid>
{
    /// <summary>
    /// Foydalanuvchi identifikatori.
    /// Идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Token qiymati (Base64).
    /// Значение токена.
    /// </summary>
    public string Token { get; private set; } = default!;

    /// <summary>
    /// Muddati tugash sanasi.
    /// Дата истечения.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Bekor qilinganmi.
    /// Отозван ли.
    /// </summary>
    public bool IsRevoked { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive  => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, int expiryDays = 30)
        => new()
        {
            Id        = Guid.NewGuid(),
            UserId    = userId,
            Token     = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };

    public void Revoke() => IsRevoked = true;
}
