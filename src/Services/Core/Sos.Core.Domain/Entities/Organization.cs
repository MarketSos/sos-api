using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

public class Organization : LocalizableEntity<Guid>
{
    public string?            Slug        { get; private set; }
    public string?            Code        { get; set; }
    public string?            Tin         { get; set; }
    public string?            Okonx       { get; set; }
    public string?            Oked        { get; set; }
    public OrganizationType?  OrgType     { get; set; }
    public bool               IsActive    { get; private set; } = true;
    public bool               IsTest      { get; set; }
    public Guid               OwnerUserId { get; private set; }
    public Guid?              ParentId    { get; private set; }

    public virtual Organization?             Parent { get; private set; }
    public virtual ICollection<Organization> Childs { get; private set; } = [];

    public Guid?           AddressId { get; private set; }
    public virtual Address? Address  { get; set; }

    public IReadOnlyCollection<OrganizationMember> Members => _members.AsReadOnly();
    private readonly List<OrganizationMember> _members = [];

    private Organization() { }

    public static Organization Create(
        Guid    id,
        string  nameUz,
        string  nameRu,
        string  slug,
        Guid    ownerUserId,
        string? nameEn      = null,
        string? nameUzKiril = null)
    {
        var org = new Organization
        {
            Id          = id,
            Slug        = slug.Trim().ToLowerInvariant(),
            OwnerUserId = ownerUserId
        };
        org.SetNames(nameUz, nameRu, nameEn, nameUzKiril);
        org._members.Add(OrganizationMember.Create(Guid.NewGuid(), id, ownerUserId, OrganizationRole.Owner));
        return org;
    }

    public void UpdateNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzKiril = null)
        => SetNames(nameUz, nameRu, nameEn, nameUzKiril);

    public void SetParent(Guid parentId)  => ParentId  = parentId;
    public void RemoveParent()            => ParentId  = null;
    public void SetAddress(Guid id)       => AddressId = id;
    public void RemoveAddress()           => AddressId = null;
    public void Activate()                => IsActive  = true;
    public void Deactivate()              => IsActive  = false;

    public OrganizationMember AddMember(Guid userId, OrganizationRole role = OrganizationRole.Member)
    {
        var existing = _members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
        if (existing is not null) return existing;

        var member = OrganizationMember.Create(Guid.NewGuid(), Id, userId, role);
        _members.Add(member);
        return member;
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
        member?.Deactivate();
    }
}
