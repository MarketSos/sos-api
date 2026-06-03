using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// SKU — единица складского учёта, партия товара при поступлении.
/// </summary>
public class Sku : AggregateRoot<Guid>
{
    /// <summary>
    /// Серийный номер партии — номер приходного документа или лота (уникальный)
    /// </summary>
    public string SerialNumber { get; private set; } = default!;

    /// <summary>
    /// Идентификатор товара
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Идентификатор поставщика (необязательно)
    /// </summary>
    public Guid? SupplierId { get; private set; }

    /// <summary>
    /// Единица измерения для данной партии (шт, кг, коробка ...)
    /// </summary>
    public Guid MeasurementUnitId { get; private set; }

    /// <summary>
    /// Количество при поступлении
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Вес партии (кг), необязательно
    /// </summary>
    public decimal? Weight { get; private set; }

    /// <summary>
    /// Закупочная цена данной партии
    /// </summary>
    public decimal CostPrice { get; private set; }

    /// <summary>
    /// Розничная цена данной партии
    /// </summary>
    public decimal SalePrice { get; private set; }

    /// <summary>
    /// Срок годности (для продуктов питания, медикаментов и т.д.)
    /// </summary>
    public DateTimeOffset? ExpirationDate { get; private set; }

    /// <summary>
    /// Дата последнего расхода
    /// </summary>
    public DateTimeOffset? LastOutcomeDate { get; private set; }

    /// <summary>
    /// Статус
    /// </summary>
    public SkuStatus Status { get; private set; } = SkuStatus.Active;

    public Product?         Product         { get; private set; }
    public MeasurementUnit? MeasurementUnit { get; private set; }

    private Sku() { }

    public static Sku Create(
        Guid    productId,
        string  serialNumber,
        Guid    measurementUnitId,
        decimal amount,
        decimal costPrice,
        decimal salePrice,
        Guid?   supplierId            = null,
        decimal? weight               = null,
        DateTimeOffset? expirationDate = null)
    {
        return new Sku
        {
            Id                = Guid.NewGuid(),
            ProductId         = productId,
            SerialNumber      = serialNumber.Trim(),
            MeasurementUnitId = measurementUnitId,
            Amount            = amount,
            CostPrice         = costPrice,
            SalePrice         = salePrice,
            SupplierId        = supplierId,
            Weight            = weight,
            ExpirationDate    = expirationDate,
            Status            = SkuStatus.Active
        };
    }

    /// <summary>
    /// Списать количество при продаже или расходе
    /// </summary>
    public Result Deduct(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure("Количество должно быть больше 0.");
        if (Amount < amount)
            return Result.Failure($"Недостаточный остаток. Доступно: {Amount}");

        Amount         -= amount;
        LastOutcomeDate = DateTimeOffset.UtcNow;

        if (Amount == 0)
            Status = SkuStatus.Depleted;

        return Result.Success();
    }

    /// <summary>
    /// Обновить розничную цену
    /// </summary>
    public void UpdateSalePrice(decimal newPrice)
    {
        SalePrice = newPrice;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Пометить как просроченный
    /// </summary>
    public void MarkExpired()
    {
        Status    = SkuStatus.Expired;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Скорректировать количество (инвентаризация)
    /// </summary>
    public void Adjust(decimal newAmount)
    {
        Amount    = newAmount;
        Status    = newAmount > 0 ? SkuStatus.Active : SkuStatus.Depleted;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Истёк ли срок годности
    /// </summary>
    public bool IsExpired =>
        ExpirationDate.HasValue && ExpirationDate.Value < DateTimeOffset.UtcNow;
}

/// <summary>
/// Статус SKU
/// </summary>
public enum SkuStatus
{
    /// <summary>
    /// Активен — есть остаток
    /// </summary>
    Active = 1,

    /// <summary>
    /// Остаток исчерпан
    /// </summary>
    Depleted = 2,

    /// <summary>
    /// Срок годности истёк
    /// </summary>
    Expired = 3,

    /// <summary>
    /// Возвращён поставщику
    /// </summary>
    Returned = 4
}
