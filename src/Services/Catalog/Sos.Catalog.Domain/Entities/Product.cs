using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

public class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string Barcode { get; private set; } = default!;
    public string? SKU { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? BrandId { get; private set; }
    public decimal BasePrice { get; private set; }
    public decimal? CostPrice { get; private set; }
    public string? ImageUrl { get; private set; }
    public decimal? Weight { get; private set; }
    public string? Unit { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsWeighed { get; private set; }

    private Product() { }

    public static Product Create(Guid id, string name, string barcode, Guid categoryId,
        decimal basePrice, string? sku = null, string? unit = "dona")
    {
        var product = new Product
        {
            Id = id,
            Name = name,
            Barcode = barcode,
            CategoryId = categoryId,
            BasePrice = basePrice,
            SKU = sku,
            Unit = unit
        };
        product.AddDomainEvent(new ProductCreatedDomainEvent(product));
        return product;
    }

    public void UpdatePrice(decimal newPrice)
    {
        BasePrice = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
}
