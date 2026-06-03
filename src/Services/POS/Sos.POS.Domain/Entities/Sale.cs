using Sos.Shared.Kernel.Domain;

namespace Sos.POS.Domain.Entities;

/// <summary>
/// Продажа (чек) — главный агрегат POS-системы.
/// Содержит позиции, способ оплаты и итоговые суммы.
/// </summary>
public class Sale : AggregateRoot<Guid>
{
    /// <summary>
    /// ID магазина, где совершена продажа
    /// </summary>
    public Guid StoreId { get; private set; }

    /// <summary>
    /// ID кассира
    /// </summary>
    public Guid CashierId { get; private set; }

    /// <summary>
    /// ID покупателя (если идентифицирован — для бонусов)
    /// </summary>
    public Guid? CustomerId { get; private set; }

    /// <summary>
    /// Статус продажи
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

    private readonly List<SaleItem> _items = [];

    /// <summary>
    /// Позиции в чеке
    /// </summary>
    public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();

    private Sale() { }

    /// <summary>
    /// Открыть новый чек на кассе
    /// </summary>
    public static Sale Create(Guid storeId, Guid cashierId, Guid? customerId = null)
        => new() { Id = Guid.NewGuid(), StoreId = storeId, CashierId = cashierId, CustomerId = customerId, Status = SaleStatus.Draft };

    /// <summary>
    /// Добавить товар в чек (или увеличить количество если уже есть)
    /// </summary>
    public void AddItem(Guid productId, string productName, int qty, decimal unitPrice, decimal discount = 0)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null) { existing.IncreaseQuantity(qty); }
        else { _items.Add(SaleItem.Create(Id, productId, productName, qty, unitPrice, discount)); }
        RecalculateTotals();
    }

    /// <summary>
    /// Убрать товар из чека
    /// </summary>
    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null) { _items.Remove(item); RecalculateTotals(); }
    }

    /// <summary>
    /// Завершить продажу и сформировать чек
    /// </summary>
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

    /// <summary>
    /// Отменить продажу
    /// </summary>
    public void Cancel() { Status = SaleStatus.Cancelled; UpdatedAt = DateTime.UtcNow; }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(i => i.TotalPrice);
        DiscountAmount = _items.Sum(i => i.DiscountAmount);
        TaxAmount = SubTotal * 0.12m;
        TotalAmount = SubTotal + TaxAmount - DiscountAmount;
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
    /// Возврат оформлен
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
