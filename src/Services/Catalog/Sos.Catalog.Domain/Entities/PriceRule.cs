using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Mahsulot narx qoidasi (aksiya, chegirma).
/// Правило цены для товара (акция, скидка).
/// </summary>
public class PriceRule : AggregateRoot<Guid>, IHasOrganization
{
    /// <summary>
    /// Mahsulot ID.
    /// ID товара.
    /// </summary>
    public Guid OrganizationId { get; set; }
    public Guid ProductId      { get; private set; }

    /// <summary>
    /// Do'kon ID (null = barcha do'konlar).
    /// ID магазина (null = все магазины).
    /// </summary>
    public Guid? StoreId { get; private set; }

    /// <summary>
    /// Qat'iy narx (0 bo'lsa chegirma ishlatiladi).
    /// Фиксированная цена (0 — используется скидка).
    /// </summary>
    public decimal FixedPrice { get; private set; }

    /// <summary>
    /// Chegirma foizi (0–100).
    /// Процент скидки (0–100).
    /// </summary>
    public decimal DiscountPct { get; private set; }

    public DateTimeOffset  StartsAt { get; private set; }
    public DateTimeOffset? EndsAt   { get; private set; }
    public bool            IsActive { get; private set; }

    private PriceRule() { }

    public static PriceRule Create(
        Guid productId, decimal fixedPrice, decimal discountPct,
        DateTimeOffset startsAt, DateTimeOffset? endsAt, Guid? storeId = null)
        => new()
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

    public void Deactivate() => IsActive = false;

    public bool IsCurrentlyActive(DateTimeOffset now)
        => IsActive && StartsAt <= now && (EndsAt is null || EndsAt > now);
}
