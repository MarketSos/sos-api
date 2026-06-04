using Sos.Shared.Kernel.Domain;

namespace Sos.Analytics.Domain.Entities;

/// <summary>
/// Tugallangan sotuv snapshotini saqlaydi — hisobot uchun denormalizatsiya qilingan ma'lumot.
/// POS servisidan SaleCompleted eventi orqali to'ldiriladi.
/// </summary>
public class SaleSnapshot : Entity<Guid>
{
    public Guid    SaleId      { get; private set; }
    public Guid    StoreId     { get; private set; }
    public Guid    CashierId   { get; private set; }
    public Guid?   CustomerId  { get; private set; }
    public decimal TotalAmount { get; private set; }
    public int     ItemCount   { get; private set; }
    public DateTimeOffset CompletedAt { get; private set; }

    private SaleSnapshot() { }

    public static SaleSnapshot Create(
        Guid saleId, Guid storeId, Guid cashierId,
        decimal totalAmount, int itemCount,
        DateTimeOffset completedAt, Guid? customerId = null) => new()
    {
        Id          = Guid.NewGuid(),
        SaleId      = saleId,
        StoreId     = storeId,
        CashierId   = cashierId,
        CustomerId  = customerId,
        TotalAmount = totalAmount,
        ItemCount   = itemCount,
        CompletedAt = completedAt
    };
}
