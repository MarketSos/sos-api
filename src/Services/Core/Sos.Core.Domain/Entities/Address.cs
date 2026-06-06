using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

/// <summary>
/// Tashkilot manzili.
/// Адрес организации.
/// </summary>
public class Address : Entity<Guid>
{
    /// <summary>
    /// To'liq manzil.
    /// Полная строка адреса.
    /// </summary>
    public string  AddressLine    { get; private set; } = default!;

    /// <summary>
    /// Viloyat.
    /// Область.
    /// </summary>
    public string? Region         { get; private set; }

    /// <summary>
    /// Tuman.
    /// Район.
    /// </summary>
    public string? District       { get; private set; }

    /// <summary>
    /// Mahalla.
    /// Махалля.
    /// </summary>
    public string? Mahalla        { get; private set; }

    /// <summary>
    /// Ko'cha.
    /// Улица.
    /// </summary>
    public string? Street         { get; private set; }

    /// <summary>
    /// Mo'ljal.
    /// Ориентир.
    /// </summary>
    public string? ReferencePoint { get; private set; }

    private Address() { }

    public static Address Create(
        string  addressLine,
        string? region         = null,
        string? district       = null,
        string? mahalla        = null,
        string? street         = null,
        string? referencePoint = null)
        => new()
        {
            Id             = Guid.NewGuid(),
            AddressLine    = addressLine,
            Region         = region,
            District       = district,
            Mahalla        = mahalla,
            Street         = street,
            ReferencePoint = referencePoint
        };

    public void Update(string addressLine, string? region, string? district,
                       string? mahalla, string? street, string? referencePoint)
    {
        AddressLine    = addressLine;
        Region         = region;
        District       = district;
        Mahalla        = mahalla;
        Street         = street;
        ReferencePoint = referencePoint;
    }
}
