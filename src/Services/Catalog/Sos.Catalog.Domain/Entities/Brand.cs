using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Бренд (товар маркаси).
/// Бренд товара.
/// </summary>
public class Brand : LocalizableEntity<Guid>
{
    /// <summary>
    /// Қисқа код — тизимда ноyob.
    /// Уникальный код бренда.
    /// </summary>
    public string Code { get; private set; } = default!;

    private Brand() { }

    public static Brand Create(
        Guid    id,
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzKiril = null)
    {
        var brand = new Brand
        {
            Id   = id,
            Code = code.Trim().ToUpperInvariant()
        };
        brand.SetNames(nameUz, nameRu, nameEn, nameUzKiril);
        return brand;
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
}
