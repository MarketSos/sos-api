using Sos.Shared.Kernel.Domain;

namespace Sos.POS.Domain.Entities;

/// <summary>
/// Продажа (чек) — главный агрегат POS-системы.
/// </summary>
public class Sale : AggregateRoot<Guid>
{
    /// <summary>
    /// Идентификатор магазина
    /// </summary>
    public Guid StoreId { get; private set; }

    /// <summary>
    /// Идентификатор кассира
    /// </summary>
    public Guid CashierId { get; private set; }

    /// <summary>
    /// Идентификатор покупателя (необязательно, для бонусной программы)
    /// </summary>
    public Guid? CustomerId { get; private set; }

    /// <summary>
    /// Статус чека
    /// </summary>
    public SaleStatus Status { get; private set; }

    /// <summary>
    /// Способ оплаты
    /// </summary>
    public PaymentMethod PaymentMethod { get; private set; }

    /// <summary>
    /// Сумма до скидок и налогов
    /// </summary>
    public decimal SubTotal { get; private set; }

    /// <summary>
    /// Общая сумма скидок
    /// </summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>
    /// Сумма НДС (12%)
    /// </summary>
    public decimal TaxAmount { get; private set; }

    /// <summary>
    /// Итоговая сумма к оплате
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Сумма, оплаченная покупателем
    /// </summary>
    public decimal PaidAmount { get; private set; }

    /// <summary>
    /// Сдача
    /// </summary>
    public decimal ChangeAmount { get; private set; }

    /// <summary>
    /// Номер чека
    /// </summary>
    public string? ReceiptNumber { get; private set; }

    public ICollection<SaleItem> Items { get; private set; } = new List<SaleItem>();

    private Sale() { }

    public static Sale Create(Guid storeId, Guid cashierId, Guid? customerId = null)
        => new() { Id = Guid.NewGuid(), StoreId = storeId, CashierId = cashierId, CustomerId = customerId, Status = SaleStatus.Draft };

    /// <summary>
    /// Добавить товар в чек (или увеличить количество, если уже есть)
    /// </summary>
    public void AddItem(Guid productId, string productName, int qty, decimal unitPrice, decimal discount = 0)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null) { existing.IncreaseQuantity(qty); }
        else { Items.Add(SaleItem.Create(Id, productId, productName, qty, unitPrice, discount)); }
        RecalculateTotals();
    }

    /// <summary>
    /// Убрать товар из чека
    /// </summary>
    public void RemoveItem(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null) { Items.Remove(item); RecalculateTotals(); }
    }

    /// <summary>
    /// Завершить продажу и сформировать чек
    /// </summary>
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

    /// <summary>
    /// Отменить продажу
    /// </summary>
    public void Cancel() { Status = SaleStatus.Cancelled; UpdatedAt = DateTimeOffset.UtcNow; }

    private void RecalculateTotals()
    {
        SubTotal       = Items.Sum(i => i.TotalPrice);
        DiscountAmount = Items.Sum(i => i.DiscountAmount);
        TaxAmount      = SubTotal * 0.12m;
        TotalAmount    = SubTotal + TaxAmount - DiscountAmount;
    }
}

/// <summary>
/// Статус продажи
/// </summary>
public enum SaleStatus
{
    /// <summary>
    /// Чек открыт, товары добавляются
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Оплачен и закрыт
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Отменён кассиром
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Оформлен возврат
    /// </summary>
    Refunded = 4
}

/// <summary>
/// Способ оплаты
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Наличными
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Банковской картой
    /// </summary>
    Card = 2,

    /// <summary>
    /// QR-код (Click, Payme и др.)
    /// </summary>
    QR = 3,

    /// <summary>
    /// Смешанная оплата
    /// </summary>
    Mixed = 4
}
