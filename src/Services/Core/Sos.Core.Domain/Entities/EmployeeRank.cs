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
        string? nameUzKiril = null)
    {
        var r = new EmployeeRank { Id = id, Code = code.Trim().ToUpperInvariant() };
        r.SetNames(nameUz, nameRu, nameEn, nameUzKiril);
        return r;
    }

    public void Update(
        string  code,
        string  nameUz,
        string  nameRu,
        string? nameEn      = null,
        string? nameUzKiril = null)
    {
        Code = code.Trim().ToUpperInvariant();
        SetNames(nameUz, nameRu, nameEn, nameUzKiril);
    }
}
