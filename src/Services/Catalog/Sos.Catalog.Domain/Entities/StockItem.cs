using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Ombordagi tovar qoldig'i.
/// Складской остаток товара.
/// </summary>
public class StockItem : AggregateRoot<Guid>, IHasOrganization
{
    /// <summary>
    /// Mahsulot ID.
    /// ID товара.
    /// </summary>
    public Guid OrganizationId { get; set; }
    public Guid ProductId      { get; private set; }

    /// <summary>
    /// Do'kon ID.
    /// ID магазина.
    /// </summary>
    public Guid StoreId { get; private set; }

    /// <summary>
    /// Joriy miqdor.
    /// Текущий остаток.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Minimal miqdor (qayta buyurtma chegarasi).
    /// Минимальный остаток (порог для дозаказа).
    /// </summary>
    public int MinQuantity { get; private set; }

    /// <summary>
    /// Maksimal miqdor.
    /// Максимальный остаток.
    /// </summary>
    public int? MaxQuantity { get; private set; }

    /// <summary>
    /// Ombordagi joylashuv.
    /// Место на складе.
    /// </summary>
    public string? Location { get; private set; }

    // Navigation
    public Store? Store { get; private set; }

    private StockItem() { }

    public static StockItem Create(Guid productId, Guid storeId, int qty, int minQty = 0)
        => new()
        {
            Id          = Guid.NewGuid(),
            ProductId   = productId,
            StoreId     = storeId,
            Quantity    = qty,
            MinQuantity = minQty
        };

    /// <summary>
    /// Sotuvda hisobdan chiqarish.
    /// Списание при продаже.
    /// </summary>
    public Result Deduct(int amount)
    {
        if (Quantity < amount)
            return Result.Failure($"Qoldiq yetarli emas. Mavjud: {Quantity}");
        Quantity  -= amount;
        UpdatedAt  = DateTimeOffset.UtcNow;
        AddDomainEvent(new StockDeductedDomainEvent(this));
        return Result.Success();
    }

    /// <summary>
    /// Tovar qabul qilish.
    /// Поступление товара.
    /// </summary>
    public void Add(int amount) { Quantity += amount; UpdatedAt = DateTimeOffset.UtcNow; }

    public void SetMinQuantity(int minQty) { MinQuantity = minQty; UpdatedAt = DateTimeOffset.UtcNow; }

    public bool IsLow => Quantity <= MinQuantity;
}

public record StockDeductedDomainEvent(StockItem StockItem) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
