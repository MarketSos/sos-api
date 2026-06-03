using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Товар в каталоге магазина. Содержит всю коммерческую информацию о продукте.
/// </summary>
public class Product : DescribedEntity<Guid>
{
    /// <summary>
    /// Штрих-код (EAN-13, QR и др.)
    /// </summary>
    public string Barcode { get; private set; } = default!;

    /// <summary>
    /// Артикул (Stock Keeping Unit)
    /// </summary>
    public string? SKU { get; private set; }

    /// <summary>
    /// ID категории товара
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// ID бренда (необязательно)
    /// </summary>
    public Guid? BrandId { get; private set; }

    /// <summary>
    /// Базовая цена продажи
    /// </summary>
    public decimal BasePrice { get; private set; }

    /// <summary>
    /// Закупочная цена (себестоимость)
    /// </summary>
    public decimal? CostPrice { get; private set; }

    /// <summary>
    /// URL изображения товара
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Вес товара (в кг)
    /// </summary>
    public decimal? Weight { get; private set; }

    /// <summary>
    /// Единица измерения (шт, кг, л и т.д.)
    /// </summary>
    public string? Unit { get; private set; }

    /// <summary>
    /// Активен ли товар в каталоге
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Продаётся ли товар на вес
    /// </summary>
    public bool IsWeighed { get; private set; }

    private Product() { }

    /// <summary>
    /// Создать новый товар в каталоге
    /// </summary>
    public static Product Create(Guid id, string nameUz, string nameRu, string barcode,
        Guid categoryId, decimal basePrice, string? nameEn = null,
        string? nameUzKiril = null, string? sku = null, string? unit = "dona")
    {
        var product = new Product
        {
            Id         = id,
            Barcode    = barcode,
            CategoryId = categoryId,
            BasePrice  = basePrice,
            SKU        = sku,
            Unit       = unit
        };
        product.SetNames(nameUz, nameRu, nameEn, nameUzKiril);
        product.AddDomainEvent(new ProductCreatedDomainEvent(product));
        return product;
    }

    /// <summary>
    /// Изменить цену товара
    /// </summary>
    public void UpdatePrice(decimal newPrice) { BasePrice = newPrice; UpdatedAt = DateTime.UtcNow; }

    /// <summary>
    /// Обновить названия товара
    /// </summary>
    public void UpdateNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzKiril = null)
        => SetNames(nameUz, nameRu, nameEn, nameUzKiril);

    /// <summary>
    /// Снять товар с продажи
    /// </summary>
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
}
