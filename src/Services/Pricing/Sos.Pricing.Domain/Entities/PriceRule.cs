using Sos.Shared.Kernel.Domain;

namespace Sos.Pricing.Domain.Entities;

/// <summary>
/// Mahsulot uchun narx qoidasi.
/// Vaqtinchalik aksiya, chegirma yoki maxsus narxni ifodalaydi.
/// </summary>
public class PriceRule : AggregateRoot<Guid>
{
    public Guid    ProductId   { get; private set; }
    public Guid?   StoreId     { get; private set; }  // null = barcha do'konlar
    public decimal FixedPrice  { get; private set; }  // 0 bo'lsa chegirma ishlatiladi
    public decimal DiscountPct { get; private set; }  // 0–100
    public DateTimeOffset StartsAt  { get; private set; }
    public DateTimeOffset? EndsAt   { get; private set; }
    public bool   IsActive     { get; private set; }

    private PriceRule() { }

    public static PriceRule Create(
        Guid productId,
        decimal fixedPrice,
        decimal discountPct,
        DateTimeOffset startsAt,
        DateTimeOffset? endsAt,
        Guid? storeId = null)
    {
        return new PriceRule
        {
            Id          = Guid.NewGuid(),
            ProductId   = productId,
            StoreId     = storeId,
            FixedPrice  = fixedPrice,
            DiscountPct = discountPct,
            StartsAt    = startsAt,
            EndsAt      = endsAt,
            IsActive    = true
        };
    }

    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Hozirgi vaqtda qoida amaldami?
    /// </summary>
    public bool IsCurrentlyActive(DateTimeOffset now) =>
        IsActive && StartsAt <= now && (EndsAt is null || EndsAt > now);
}
