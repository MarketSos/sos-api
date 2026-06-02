using Sos.Shared.Kernel.Domain;

namespace Sos.Identity.Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? StoreId { get; private set; }

    private User() { }

    public static User Create(Guid id, string email, string passwordHash,
        string firstName, string lastName, UserRole role, Guid? storeId = null)
    {
        return new User
        {
            Id = id,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            StoreId = storeId
        };
    }

    public void Deactivate() => IsActive = false;
    public void UpdatePassword(string hash) { PasswordHash = hash; UpdatedAt = DateTime.UtcNow; }
}
