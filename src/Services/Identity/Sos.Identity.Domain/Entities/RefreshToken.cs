using Sos.Shared.Kernel.Domain;

namespace Sos.Identity.Domain.Entities;

/// <summary>
/// Токен обновления для бессрочной авторизации без повторного входа.
/// Хранится в базе; при использовании — заменяется новым (rotation).
/// </summary>
public class RefreshToken : Entity<Guid>
{
    /// <summary>
    /// ID пользователя-владельца токена
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Значение токена (случайная строка Base64)
    /// </summary>
    public string Token { get; private set; } = default!;

    /// <summary>
    /// Дата истечения срока действия
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Отозван ли токен вручную
    /// </summary>
    public bool IsRevoked { get; private set; }

    /// <summary>
    /// Истёк ли срок действия
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Действителен ли токен (не отозван и не истёк)
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }

    /// <summary>
    /// Создать новый refresh-токен
    /// </summary>
    public static RefreshToken Create(Guid userId, string token, int expiryDays = 30)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };

    /// <summary>
    /// Отозвать токен (logout или ротация)
    /// </summary>
    public void Revoke() => IsRevoked = true;
}
