using Microsoft.AspNetCore.Identity;

namespace Sos.Core.Domain.Entities.Identity;

public class Role : IdentityRole<Guid>
{
    public string  NameUz     { get; set; } = default!;
    public string  NameUzCyrl { get; set; } = default!;
    public string  NameRu     { get; set; } = default!;
    public string? NameEn     { get; set; }
    public string? NameKk     { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RoleClaim> RoleClaims { get; set; }
}