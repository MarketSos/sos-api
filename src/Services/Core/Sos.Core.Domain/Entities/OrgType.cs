using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

/// <summary>
/// Tashkilot biznes turi — oziq-ovqat do'koni, apteka, elektronika va hokazo.
/// </summary>
public class OrgType : Entity<Guid>
{
    public string  Code   { get; private set; } = default!;
    public string  NameUz { get; private set; } = default!;
    public string  NameRu { get; private set; } = default!;
    public string? NameEn { get; private set; }
    public string? Icon   { get; private set; }  // pi-* icon yoki emoji

    private OrgType() { }

    public static OrgType Create(Guid id, string code, string nameUz, string nameRu,
        string? nameEn = null, string? icon = null)
        => new()
        {
            Id     = id,
            Code   = code.ToUpperInvariant(),
            NameUz = nameUz,
            NameRu = nameRu,
            NameEn = nameEn,
            Icon   = icon
        };

    public void Update(string nameUz, string nameRu, string? nameEn = null, string? icon = null)
    {
        NameUz = nameUz;
        NameRu = nameRu;
        NameEn = nameEn;
        Icon   = icon;
    }
}
