using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Единица измерения товара.
/// </summary>
public class MeasurementUnit : CatalogEntity<Guid>
{
    /// <summary>
    /// Краткий код — используется в системе (dona, kg, l, m, box ...)
    /// </summary>
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Продаётся ли товар на вес (весовой товар)
    /// </summary>
    public bool IsWeightBased { get; private set; }

    private MeasurementUnit() { }

    public static MeasurementUnit Create(
        Guid id,
        string code,
        string nameUz,
        string nameRu,
        string? nameEn    = null,
        bool isWeightBased = false)
    {
        var unit = new MeasurementUnit
        {
            Id            = id,
            Code          = code.ToLowerInvariant(),
            IsWeightBased = isWeightBased
        };
        unit.SetNames(nameUz, nameRu, nameEn);
        return unit;
    }

    public void Update(string code, string nameUz, string nameRu, string? nameEn = null, bool isWeightBased = false)
    {
        Code          = code.ToLowerInvariant();
        IsWeightBased = isWeightBased;
        SetNames(nameUz, nameRu, nameEn);
    }
}
