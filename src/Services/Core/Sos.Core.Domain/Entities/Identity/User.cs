using Microsoft.AspNetCore.Identity;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities.Identity;

public class User : IdentityUser<Guid>, IHasOrganization
{
    public bool  IsActive       { get; private set; } = true;
    public Guid? StoreId        { get; private set; }
    public Guid  OrganizationId { get; set; }

    public User() { }

    public static User Create(
        Guid   id,
        string userName,
        string email,
        Guid?  storeId        = null,
        Guid?  organizationId = null)
    {
        var user = new User
        {
            Id       = id,
            UserName = userName.ToLowerInvariant(),
            Email    = email.ToLowerInvariant(),
            StoreId  = storeId
        };
        if (organizationId.HasValue)
            user.OrganizationId = organizationId.Value;
        return user;
    }

    public void SetOrganization(Guid organizationId) => OrganizationId = organizationId;
    public void SetStore(Guid? storeId)              => StoreId = storeId;
    public void Activate()                           => IsActive = true;
    public void Deactivate()                         => IsActive = false;
}
