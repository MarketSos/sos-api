namespace Sos.Core.Domain.Entities.Identity;

using Microsoft.AspNetCore.Identity;

public class UserRole : IdentityUserRole<Guid>
{
    public Guid Id { get; set; }
    public virtual User? User { get; set; }
    public Guid? OrganizationId { get; set; }
    public virtual Role? Role { get; set; }
}
