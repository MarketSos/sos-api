using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

public class Organization : LocalizableEntity<Guid>
{
    public string?             Slug        { get; private set; }
    public string?             Code        { get; set; }
    public string?             Tin         { get; set; }
    public string?             Okonx       { get; set; }
    public string?             Oked        { get; set; }
    public OwnershipType     Ownership   { get; private set; }
    public OrganizationLevel  Level       { get; private set; }
    public Guid?               OrgTypeId   { get; set; }
    public bool                IsActive    { get; private set; } = true;
    public bool                IsTest      { get; set; }
    public Guid                OwnerUserId { get; private set; }
    public Guid?               ParentId    { get; private set; }

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
        string? nameUzCyrl = null,
        string? nameKk      = null)
    {
        var org = new Organization
        {
            Id          = id,
            Slug        = slug.Trim().ToLowerInvariant(),
            OwnerUserId = ownerUserId
        };
        org.SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
        org._members.Add(OrganizationMember.Create(Guid.NewGuid(), id, ownerUserId, OrganizationRole.Owner));
        return org;
    }

    // ── Klassifikatsiya ───────────────────────────────────────────────────────

    /// <summary>
    /// Tashkilot mulkchilik turini o'rnatish.
    /// System = tizim ichki tashkiloti, Customer = mijoz tashkiloti.
    /// </summary>
    public void SetType(OwnershipType type) => Ownership = type;

    /// <summary>
    /// Ierarxiya darajasini o'rnatish.
    /// Root (bosh kompaniya) → Chain (tarmoq) → Store (do'kon/filial).
    /// </summary>
    public void SetLevel(OrganizationLevel level)
    {
        Level = level;
        // Level qoidalari: Store bo'lsa parent bo'lishi shart (Chain yoki Root)
        // Bu yerda faqat qiymat saqlanadi; biznes qoidalar CommandHandler da tekshiriladi.
    }

    /// <summary>
    /// Bir vaqtda tur va darajani o'rnatish.
    /// </summary>
    public void Classify(OwnershipType type, OrganizationLevel level)
    {
        Ownership = type;
        Level     = level;
    }

    // ── Boshqa metodlar ───────────────────────────────────────────────────────

    public void UpdateNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzCyrl = null, string? nameKk = null)
        => SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);

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
