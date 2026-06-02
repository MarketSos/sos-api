using Sos.Shared.Kernel.Domain;

namespace Sos.Inventory.Domain.Entities;

public class StockItem : AggregateRoot<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid StoreId { get; private set; }
    public int Quantity { get; private set; }
    public int MinQuantity { get; private set; }
    public int? MaxQuantity { get; private set; }
    public string? Location { get; private set; }  // shelf/aisle

    private StockItem() { }

    public static StockItem Create(Guid productId, Guid storeId, int qty, int minQty = 0)
        => new() { Id = Guid.NewGuid(), ProductId = productId, StoreId = storeId, Quantity = qty, MinQuantity = minQty };

    public Result Deduct(int amount)
    {
        if (Quantity < amount)
            return Result.Failure($"Insufficient stock. Available: {Quantity}");
        Quantity -= amount;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StockDeductedDomainEvent(this));
        return Result.Success();
    }

    public void Add(int amount) { Quantity += amount; UpdatedAt = DateTime.UtcNow; }

    public bool IsLow => Quantity <= MinQuantity;
}

public class Result
{
    public bool IsSuccess { get; private init; }
    public string? Error { get; private init; }
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string err) => new() { IsSuccess = false, Error = err };
}
