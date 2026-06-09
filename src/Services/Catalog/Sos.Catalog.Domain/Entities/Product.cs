using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Товар — идентификационная карточка в каталоге.
/// </summary>
public class Product : DescribedEntity<Guid>
{
    /// <summary>
    /// Штрих-код — EAN-13, QR или внутренний формат (уникальный)
    /// </summary>
    public string Barcode { get; private set; } = default!;

    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Идентификатор бренда (необязательно)
    /// </summary>
    public Guid? BrandId { get; private set; }

    /// <summary>
    /// URL изображения
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Активен ли товар
    /// </summary>
    public bool IsActive { get; private set; } = true;

    public Category? Category { get; private set; }

    private Product() { }

    public static Product Create(
        Guid id,
        string nameUz,
        string nameRu,
        string barcode,
        Guid categoryId,
        Guid? brandId      = null,
        string? nameEn     = null,
        string? nameUzCyrl = null,
        string? nameKk     = null)
    {
        var product = new Product
        {
            Id         = id,
            Barcode    = barcode.Trim(),
            CategoryId = categoryId,
            BrandId    = brandId
        };
        product.SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
        product.AddDomainEvent(new ProductCreatedDomainEvent(product));
        return product;
    }

    public void UpdateBarcode(string barcode) => Barcode = barcode.Trim();

    public void UpdateNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzCyrl = null, string? nameKk = null)
        => SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);

    public void SetImage(string url) => ImageUrl = url;

    public void Deactivate() => IsActive = false;
    public void Activate()   => IsActive = true;
}
