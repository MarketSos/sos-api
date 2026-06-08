using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Do'kon / filial.
/// Магазин / филиал.
/// </summary>
public class Store : AggregateRoot<Guid>, IHasOrganization
{
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Qisqa kod — tashkilot ichida noyob.
    /// Уникальный код магазина в рамках организации.
    /// </summary>
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Do'kon nomi.
    /// Название магазина.
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Manzil.
    /// Адрес.
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// Telefon raqami.
    /// Номер телефона.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Faolligi.
    /// Активность.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    // Navigation
    public ICollection<StockItem> StockItems { get; private set; } = [];

    private Store() { }

    public static Store Create(
        Guid    organizationId,
        string  code,
        string  name,
        string? address = null,
        string? phone   = null)
        => new()
        {
            Id             = Guid.NewGuid(),
            OrganizationId = organizationId,
            Code           = code.Trim().ToUpperInvariant(),
            Name           = name.Trim(),
            Address        = address?.Trim(),
            Phone          = phone?.Trim()
        };

    public void Update(string code, string name, string? address, string? phone)
    {
        Code      = code.Trim().ToUpperInvariant();
        Name      = name.Trim();
        Address   = address?.Trim();
        Phone     = phone?.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()   { IsActive = true;  UpdatedAt = DateTimeOffset.UtcNow; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTimeOffset.UtcNow; }
}
