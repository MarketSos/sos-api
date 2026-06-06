using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

/// <summary>Tizim foydalanuvchisi. / Пользователь системы.</summary>
public class User : AggregateRoot<Guid>
{
    /// <summary>Email (kirish identifikatori). / Email (идентификатор входа).</summary>
    public string Email { get; private set; } = default!;

    /// <summary>Parol xeshi. / Хэш пароля.</summary>
    public string PasswordHash { get; private set; } = default!;

    /// <summary>Ism. / Имя.</summary>
    public string FirstName { get; private set; } = default!;

    /// <summary>Familiya. / Фамилия.</summary>
    public string LastName { get; private set; } = default!;

    /// <summary>Tizim roli. / Роль в системе.</summary>
    public UserRole Role { get; private set; }

    /// <summary>Faolmi. / Активен ли аккаунт.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Do'kon identifikatori (kassir/omborchi uchun). / Идентификатор магазина.</summary>
    public Guid? StoreId { get; private set; }

    private User() { }

    public static User Create(
        Guid id, string email, string passwordHash,
        string firstName, string lastName, UserRole role,
        Guid? storeId = null, Guid? organizationId = null)
    {
        var user = new User
        {
            Id           = id,
            Email        = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName    = firstName,
            LastName     = lastName,
            Role         = role,
            StoreId      = storeId
        };
        if (organizationId.HasValue)
            user.OrganizationId = organizationId.Value;
        return user;
    }

    public void SetOrganization(Guid organizationId)
    {
        OrganizationId = organizationId;
        UpdatedAt      = DateTimeOffset.UtcNow;
    }

    public void Deactivate() => IsActive = false;

    public void UpdatePassword(string hash)
    {
        PasswordHash = hash;
        UpdatedAt    = DateTimeOffset.UtcNow;
    }
}
