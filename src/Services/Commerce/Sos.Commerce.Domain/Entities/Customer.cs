using Sos.Shared.Kernel.Domain;

namespace Sos.Commerce.Domain.Entities;

/// <summary>
/// Do'kon mijozi.
/// Покупатель магазина.
/// </summary>
public class Customer : AggregateRoot<Guid>, IHasOrganization
{
    public Guid      OrganizationId { get; set; }
    public string    FirstName      { get; private set; } = default!;
    public string    LastName    { get; private set; } = default!;
    public string?   PhoneNumber { get; private set; }
    public string?   Email       { get; private set; }
    public DateOnly? BirthDate   { get; private set; }
    public Guid?     StoreId     { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private Customer() { }

    public static Customer Create(
        string firstName, string lastName,
        string? phoneNumber, string? email,
        DateOnly? birthDate, Guid? storeId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName bo'sh bo'lishi mumkin emas.");

        return new Customer
        {
            Id          = Guid.NewGuid(),
            FirstName   = firstName.Trim(),
            LastName    = lastName.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            Email       = email?.Trim().ToLowerInvariant(),
            BirthDate   = birthDate,
            StoreId     = storeId
        };
    }

    public void Update(string firstName, string lastName, string? phoneNumber, string? email, DateOnly? birthDate)
    {
        FirstName   = firstName.Trim();
        LastName    = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Email       = email?.Trim().ToLowerInvariant();
        BirthDate   = birthDate;
        UpdatedAt   = DateTimeOffset.UtcNow;
    }
}
