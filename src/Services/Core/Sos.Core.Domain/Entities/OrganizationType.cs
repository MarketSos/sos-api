using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

/// <summary>
/// Tashkilot biznes turi — oziq-ovqat do'koni, apteka, elektronika va hokazo.
/// </summary>
public class OrganizationType : LocalizableEntity<Guid>
{
    public string  Code { get; private set; } = default!;
    public string? Icon { get; private set; }  // pi-* icon yoki emoji

    private OrganizationType() { }

    public static OrganizationType Create(Guid id, string code, string nameUz, string nameRu,
        string? nameEn = null, string? nameUzCyrl = null, string? nameKk = null, string? icon = null)
        => new()
        {
            Id          = id,
            Code        = code.ToUpperInvariant(),
            NameUz      = nameUz,
            NameRu      = nameRu,
            NameEn      = nameEn,
            NameUzCyrl  = nameUzCyrl,
            NameKk      = nameKk,
            Icon        = icon
        };

    public void Update(string nameUz, string nameRu, string? nameEn = null, string? nameUzCyrl = null, string? nameKk = null, string? icon = null)
    {
        NameUz     = nameUz;
        NameRu     = nameRu;
        NameEn     = nameEn;
        NameUzCyrl = nameUzCyrl;
        NameKk     = nameKk;
        Icon       = icon;
    }
}
