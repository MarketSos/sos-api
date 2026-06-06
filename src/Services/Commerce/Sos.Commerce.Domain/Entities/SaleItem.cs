using Sos.Shared.Kernel.Domain;

namespace Sos.Commerce.Domain.Entities;

/// <summary>
/// Chek satri.
/// Позиция в чеке.
/// </summary>
public class SaleItem : Entity<Guid>
{
    public Guid    SaleId         { get; private set; }
    public Guid    ProductId      { get; private set; }
    public string  ProductName    { get; private set; } = default!;
    public int     Quantity       { get; private set; }
    public decimal UnitPrice      { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalPrice => (UnitPrice * Quantity) - DiscountAmount;

    private SaleItem() { }

    public static SaleItem Create(Guid saleId, Guid productId, string name, int qty, decimal price, decimal discount = 0)
        => new()
        {
            Id             = Guid.NewGuid(),
            SaleId         = saleId,
            ProductId      = productId,
            ProductName    = name,
            Quantity       = qty,
            UnitPrice      = price,
            DiscountAmount = discount
        };

    public void IncreaseQuantity(int qty) => Quantity += qty;
}
