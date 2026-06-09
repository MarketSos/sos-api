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
        string? nameUzCyrl = null,
        string? nameKk      = null)
    {
        var brand = new Brand
        {
            Id   = id,
            Code = code.Trim().ToUpperInvariant()
        };
        brand.SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
        return brand;
    }

    public void Update(
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzCyrl = null,
        string? nameKk      = null)
    {
        Code = code.Trim().ToUpperInvariant();
        SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
    }
}
