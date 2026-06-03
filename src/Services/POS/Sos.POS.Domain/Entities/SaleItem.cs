using Sos.Shared.Kernel.Domain;

namespace Sos.POS.Domain.Entities;

/// <summary>
/// Позиция в чеке — товар с количеством и ценой.
/// </summary>
public class SaleItem : Entity<Guid>
{
    /// <summary>
    /// Идентификатор чека
    /// </summary>
    public Guid SaleId { get; private set; }

    /// <summary>
    /// Идентификатор товара
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Название товара (фиксируется на момент продажи)
    /// </summary>
    public string ProductName { get; private set; } = default!;

    /// <summary>
    /// Количество
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Цена за единицу
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Скидка по позиции
    /// </summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>
    /// Итого по позиции (цена × количество − скидка)
    /// </summary>
    public decimal TotalPrice => (UnitPrice * Quantity) - DiscountAmount;

    private SaleItem() { }

    public static SaleItem Create(Guid saleId, Guid productId, string name, int qty, decimal price, decimal discount = 0)
        => new() { Id = Guid.NewGuid(), SaleId = saleId, ProductId = productId, ProductName = name, Quantity = qty, UnitPrice = price, DiscountAmount = discount };

    /// <summary>
    /// Увеличить количество при повторном добавлении того же товара
    /// </summary>
    public void IncreaseQuantity(int qty) => Quantity += qty;
}
