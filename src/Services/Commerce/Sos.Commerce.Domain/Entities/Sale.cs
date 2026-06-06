using Sos.Shared.Kernel.Domain;

namespace Sos.Commerce.Domain.Entities;

/// <summary>Sotuv (chek) — POS asosiy agregati. / Продажа (чек).</summary>
public class Sale : AggregateRoot<Guid>
{
    public Guid          StoreId       { get; private set; }
    public Guid          CashierId     { get; private set; }
    public Guid?         CustomerId    { get; private set; }
    public SaleStatus    Status        { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public decimal       SubTotal      { get; private set; }
    public decimal       DiscountAmount{ get; private set; }
    public decimal       TaxAmount     { get; private set; }
    public decimal       TotalAmount   { get; private set; }
    public decimal       PaidAmount    { get; private set; }
    public decimal       ChangeAmount  { get; private set; }
    public string?       ReceiptNumber { get; private set; }

    public ICollection<SaleItem> Items { get; private set; } = new List<SaleItem>();

    private Sale() { }

    public static Sale Create(Guid storeId, Guid cashierId, Guid? customerId = null)
        => new()
        {
            Id         = Guid.NewGuid(),
            StoreId    = storeId,
            CashierId  = cashierId,
            CustomerId = customerId,
            Status     = SaleStatus.Draft
        };

    public void AddItem(Guid productId, string productName, int qty, decimal unitPrice, decimal discount = 0)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null) existing.IncreaseQuantity(qty);
        else Items.Add(SaleItem.Create(Id, productId, productName, qty, unitPrice, discount));
        RecalculateTotals();
    }

    public void RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null) { Items.Remove(item); RecalculateTotals(); }
    }

    public void Complete(PaymentMethod method, decimal paidAmount)
    {
        Status        = SaleStatus.Completed;
        PaymentMethod = method;
        PaidAmount    = paidAmount;
        ChangeAmount  = paidAmount - TotalAmount;
        ReceiptNumber = $"RCP-{DateTime.UtcNow:yyyyMMdd}-{Id.ToString()[..8].ToUpper()}";
        UpdatedAt     = DateTimeOffset.UtcNow;
        AddDomainEvent(new SaleCompletedDomainEvent(this));
    }

    public void Cancel() { Status = SaleStatus.Cancelled; UpdatedAt = DateTimeOffset.UtcNow; }

    private void RecalculateTotals()
    {
        SubTotal       = Items.Sum(i => i.TotalPrice);
        DiscountAmount = Items.Sum(i => i.DiscountAmount);
        TaxAmount      = SubTotal * 0.12m;
        TotalAmount    = SubTotal + TaxAmount - DiscountAmount;
    }
}

public enum SaleStatus    { Draft = 1, Completed = 2, Cancelled = 3, Refunded = 4 }
public enum PaymentMethod { Cash = 1, Card = 2, QR = 3, Mixed = 4 }

public record SaleCompletedDomainEvent(Sale Sale) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
