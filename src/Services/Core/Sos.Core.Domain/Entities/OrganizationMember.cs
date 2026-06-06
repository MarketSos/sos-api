using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

/// <summary>
/// Tashkilot a'zosi.
/// Участник организации.
/// </summary>
public class OrganizationMember : Entity<Guid>, IHasOrganization
{
    public Guid             OrganizationId { get; set; }
    public virtual Organization? Organization { get; set; }
    public Guid             UserId         { get; private set; }
    public OrganizationRole Role           { get; private set; }
    public DateTimeOffset   JoinedAt       { get; private set; }
    public bool             IsActive       { get; private set; } = true;

    private OrganizationMember() { }

    internal static OrganizationMember Create(Guid id, Guid organizationId, Guid userId, OrganizationRole role)
        => new()
        {
            Id             = id,
            OrganizationId = organizationId,
            UserId         = userId,
            Role           = role,
            JoinedAt       = DateTimeOffset.UtcNow
        };

    public void ChangeRole(OrganizationRole role) => Role = role;
    public void Deactivate()                       => IsActive = false;
}
