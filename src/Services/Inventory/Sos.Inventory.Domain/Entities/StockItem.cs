using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Inventory.Domain.Entities;

/// <summary>
/// Складская позиция — остаток конкретного товара в конкретном магазине.
/// </summary>
public class StockItem : AggregateRoot<Guid>
{
    /// <summary>
    /// ID товара из каталога
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// ID магазина
    /// </summary>
    public Guid StoreId { get; private set; }

    /// <summary>
    /// Текущее количество на складе
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Минимальный остаток (порог для уведомления)
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

    /// <summary>
    /// Создать складскую позицию
    /// </summary>
    public static StockItem Create(Guid productId, Guid storeId, int qty, int minQty = 0)
        => new() { Id = Guid.NewGuid(), ProductId = productId, StoreId = storeId, Quantity = qty, MinQuantity = minQty };

    /// <summary>
    /// Списать товар со склада (при продаже).
    /// Возвращает ошибку если остатка недостаточно.
    /// </summary>
    public Result Deduct(int amount)
    {
        if (Quantity < amount)
            return Result.Failure($"Insufficient stock. Available: {Quantity}");
        Quantity -= amount;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StockDeductedDomainEvent(this));
        return Result.Success();
    }

    /// <summary>
    /// Добавить товар на склад (поступление)
    /// </summary>
    public void Add(int amount) { Quantity += amount; UpdatedAt = DateTime.UtcNow; }

    /// <summary>
    /// Остаток ниже минимального порога
    /// </summary>
    public bool IsLow => Quantity <= MinQuantity;
}
