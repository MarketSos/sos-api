using Sos.Shared.Kernel.Domain;

namespace Sos.Identity.Domain.Entities;

/// <summary>
/// Refresh-токен — при использовании заменяется новым (rotation).
/// </summary>
public class RefreshToken : Entity<Guid>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Значение токена (случайная строка Base64)
    /// </summary>
    public string Token { get; private set; } = default!;

    /// <summary>
    /// Дата истечения
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Отозван ли вручную
    /// </summary>
    public bool IsRevoked { get; private set; }

    /// <summary>
    /// Истёк ли срок действия
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Действителен ли токен
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, int expiryDays = 30)
        => new()
        {
            Id        = Guid.NewGuid(),
            UserId    = userId,
            Token     = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };

    /// <summary>
    /// Отозвать токен (выход или ротация)
    /// </summary>
    public void Revoke() => IsRevoked = true;
}
