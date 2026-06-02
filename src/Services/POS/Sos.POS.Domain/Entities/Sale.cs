using Sos.Shared.Kernel.Domain;

namespace Sos.POS.Domain.Entities;

public class Sale : AggregateRoot<Guid>
{
    public Guid StoreId { get; private set; }
    public Guid CashierId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public SaleStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal ChangeAmount { get; private set; }
    public string? ReceiptNumber { get; private set; }
    private readonly List<SaleItem> _items = [];
    public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();

    private Sale() { }

    public static Sale Create(Guid storeId, Guid cashierId, Guid? customerId = null)
        => new() { Id = Guid.NewGuid(), StoreId = storeId, CashierId = cashierId, CustomerId = customerId, Status = SaleStatus.Draft };

    public void AddItem(Guid productId, string productName, int qty, decimal unitPrice, decimal discount = 0)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null) { existing.IncreaseQuantity(qty); }
        else { _items.Add(SaleItem.Create(Id, productId, productName, qty, unitPrice, discount)); }
        RecalculateTotals();
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null) { _items.Remove(item); RecalculateTotals(); }
    }

    public void Complete(PaymentMethod method, decimal paidAmount)
    {
        Status = SaleStatus.Completed;
        PaymentMethod = method;
        PaidAmount = paidAmount;
        ChangeAmount = paidAmount - TotalAmount;
        ReceiptNumber = $"RCP-{DateTime.UtcNow:yyyyMMdd}-{Id.ToString()[..8].ToUpper()}";
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleCompletedDomainEvent(this));
    }

    public void Cancel() { Status = SaleStatus.Cancelled; UpdatedAt = DateTime.UtcNow; }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(i => i.TotalPrice);
        DiscountAmount = _items.Sum(i => i.DiscountAmount);
        TaxAmount = SubTotal * 0.12m; // 12% QQS
        TotalAmount = SubTotal + TaxAmount - DiscountAmount;
    }
}

public enum SaleStatus { Draft, Completed, Cancelled, Refunded }
public enum PaymentMethod { Cash, Card, QR, Mixed }
