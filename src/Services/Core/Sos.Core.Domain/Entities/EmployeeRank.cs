using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

public class EmployeeRank : LocalizableEntity<Guid>, IHasOrganization
{
    public Guid OrganizationId { get; set; }
    public string Code { get; private set; } = default!;

    private EmployeeRank() { }

    public static EmployeeRank Create(
        Guid    id,
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzCyrl = null,
        string? nameKk      = null)
    {
        var r = new EmployeeRank { Id = id, Code = code.Trim().ToUpperInvariant() };
        r.SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
        return r;
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
