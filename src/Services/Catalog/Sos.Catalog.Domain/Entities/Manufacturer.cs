using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Ишлаб чиқарувчи (завод/компания).
/// Производитель товара.
/// </summary>
public class Manufacturer : LocalizableEntity<Guid>
{
    /// <summary>
    /// Қисқа код — тизимда ноyob.
    /// Уникальный код производителя.
    /// </summary>
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Манзил.
    /// Адреса.
    /// </summary>
    public string? AddressLine { get; private set; }

    public string? Phone { get; private set; }

    private Manufacturer() { }

    public static Manufacturer Create(
        Guid    id,
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzKiril = null,
        string? addressLine = null,
        string? phone       = null)
    {
        var manufacturer = new Manufacturer
        {
            Id        = id,
            Code      = code.Trim().ToUpperInvariant(),
            AddressLine = addressLine,
            Phone = phone
        };
        manufacturer.SetNames(nameUz, nameRu, nameEn, nameUzKiril);
        return manufacturer;
    }

    public void Update(
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzKiril = null)
    {
        Code = code.Trim().ToUpperInvariant();
        SetNames(nameUz, nameRu, nameEn, nameUzKiril);
    }

    public void SetAddress(string? addressLine)
    {
        AddressLine = addressLine;
    }
    public void SetPhone(string? phone) { Phone = phone; }
}
