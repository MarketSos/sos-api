using Sos.Shared.Kernel.Domain;

namespace Sos.Identity.Domain.Entities;

/// <summary>
/// Пользователь системы.
/// </summary>
public class User : AggregateRoot<Guid>
{
    /// <summary>
    /// Email (уникальный идентификатор входа)
    /// </summary>
    public string Email { get; private set; } = default!;

    /// <summary>
    /// Хэш пароля
    /// </summary>
    public string PasswordHash { get; private set; } = default!;

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; private set; } = default!;

    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; private set; } = default!;

    /// <summary>
    /// Роль в системе
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Активен ли аккаунт
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Идентификатор магазина (для кассиров и кладовщиков)
    /// </summary>
    public Guid? StoreId { get; private set; }

    private User() { }

    public static User Create(Guid id, string email, string passwordHash,
        string firstName, string lastName, UserRole role, Guid? storeId = null)
    {
        return new User
        {
            Id           = id,
            Email        = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName    = firstName,
            LastName     = lastName,
            Role         = role,
            StoreId      = storeId
        };
    }

    /// <summary>
    /// Деактивировать аккаунт
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Обновить пароль
    /// </summary>
    public void UpdatePassword(string hash) { PasswordHash = hash; UpdatedAt = DateTimeOffset.UtcNow; }
}
