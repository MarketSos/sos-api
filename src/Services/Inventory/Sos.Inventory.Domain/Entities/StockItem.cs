using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Inventory.Domain.Entities;

/// <summary>
/// Складская позиция — остаток товара в конкретном магазине.
/// </summary>
public class StockItem : AggregateRoot<Guid>
{
    /// <summary>
    /// Идентификатор товара
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Идентификатор магазина
    /// </summary>
    public Guid StoreId { get; private set; }

    /// <summary>
    /// Текущий остаток
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Минимальный остаток (порог для уведомления о дозаказе)
    /// </summary>
    public int MinQuantity { get; private set; }

    /// <summary>
    /// Максимальный остаток (для управления заказами)
    /// </summary>
    public int? MaxQuantity { get; private set; }

    /// <summary>
    /// Местонахождение на складе (стеллаж, секция)
    /// </summary>
    public string? Location { get; private set; }

    private StockItem() { }

    public static StockItem Create(Guid productId, Guid storeId, int qty, int minQty = 0)
        => new() { Id = Guid.NewGuid(), ProductId = productId, StoreId = storeId, Quantity = qty, MinQuantity = minQty };

    /// <summary>
    /// Списать со склада при продаже
    /// </summary>
    public Result Deduct(int amount)
    {
        if (Quantity < amount)
            return Result.Failure($"Недостаточный остаток. Доступно: {Quantity}");
        Quantity -= amount;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new StockDeductedDomainEvent(this));
        return Result.Success();
    }

    /// <summary>
    /// Принять товар на склад
    /// </summary>
    public void Add(int amount) { Quantity += amount; UpdatedAt = DateTimeOffset.UtcNow; }

    /// <summary>
    /// Остаток ниже минимального порога
    /// </summary>
    public bool IsLow => Quantity <= MinQuantity;
}
