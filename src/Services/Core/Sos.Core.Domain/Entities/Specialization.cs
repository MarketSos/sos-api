using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

public class Specialization : LocalizableEntity<Guid>, IHasOrganization
{
    public Guid OrganizationId { get; set; }
    public string Code { get; private set; } = default!;

    private Specialization() { }

    public static Specialization Create(
        Guid    id,
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzCyrl = null,
        string? nameKk      = null)
    {
        var s = new Specialization { Id = id, Code = code.Trim().ToUpperInvariant() };
        s.SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
        return s;
    }

    public void Update(
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzCyrl = null,
        string? nameKk      = null)
    {
        Code = code.Trim().ToUpperInvariant();
        SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
    }
}
