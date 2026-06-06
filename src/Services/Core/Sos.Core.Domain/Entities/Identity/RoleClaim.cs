using Microsoft.AspNetCore.Identity;

namespace Sos.Core.Domain.Entities.Identity;

public class RoleClaim : IdentityRoleClaim<Guid>
{
    public virtual Role? Role { get; set; }
}
