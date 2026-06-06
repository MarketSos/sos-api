using Sos.Core.Domain.Enums;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

/// <summary>Tashkilot (tenant). / Организация (тенант).</summary>
public class Organization : LocalizableEntity<Guid>
{
    /// <summary>Unikal slug. / Уникальный slug.</summary>
    public string? Slug { get; private set; }

    /// <summary>Tashkilot kodi. / Код организации.</summary>
    public string? Code { get; set; }

    /// <summary>STIR. / ИНН.</summary>
    public string? Tin   { get; set; }

    /// <summary>OKONX. / ОКОНХ.</summary>
    public string? Okonx { get; set; }

    /// <summary>OKED. / ОКЭД.</summary>
    public string? Oked  { get; set; }

    /// <summary>Tashkilot turi. / Тип организации.</summary>
    public OrganizationType? OrgType { get; set; }

    /// <summary>Faolmi. / Активна ли.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>Test tashkilotimi. / Тестовая организация.</summary>
    public bool IsTest { get; set; }

    /// <summary>Egasi foydalanuvchi ID. / ID пользователя-владельца.</summary>
    public Guid OwnerUserId { get; private set; }

    /// <summary>Yuqori tashkilot ID. / ID родительской организации.</summary>
    public Guid? ParentId { get; private set; }

    public virtual Organization?             Parent { get; private set; }
    public virtual ICollection<Organization> Childs { get; private set; } = [];

    /// <summary>Manzil ID (1-to-1). / ID адреса.</summary>
    public Guid?           AddressId { get; private set; }
    public virtual Address? Address  { get; private set; }

    public IReadOnlyCollection<OrganizationMember> Members => _members.AsReadOnly();
    private readonly List<OrganizationMember> _members = [];

    private Organization() { }

    public static Organization Create(
        Guid id, string nameUz, string nameRu, string slug,
        Guid ownerUserId, string? nameEn = null, string? nameUzKiril = null)
    {
        var org = new Organization
        {
            Id          = id,
            Slug        = slug.Trim().ToLowerInvariant(),
            OwnerUserId = ownerUserId,
        };
        org.OrganizationId = id;
        org.SetNames(nameUz, nameRu, nameEn, nameUzKiril);
        org._members.Add(OrganizationMember.Create(Guid.NewGuid(), id, ownerUserId, OrganizationRole.Owner));
        return org;
    }

    public void UpdateNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzKiril = null)
        => SetNames(nameUz, nameRu, nameEn, nameUzKiril);

    public void SetParent(Guid parentId)   { ParentId  = parentId; UpdatedAt = DateTimeOffset.UtcNow; }
    public void RemoveParent()             { ParentId  = null;      UpdatedAt = DateTimeOffset.UtcNow; }
    public void SetAddress(Guid id)        { AddressId = id;        UpdatedAt = DateTimeOffset.UtcNow; }
    public void RemoveAddress()            { AddressId = null;      UpdatedAt = DateTimeOffset.UtcNow; }
    public void Activate()                 { IsActive  = true;      UpdatedAt = DateTimeOffset.UtcNow; }
    public void Deactivate()               { IsActive  = false;     UpdatedAt = DateTimeOffset.UtcNow; }

    public OrganizationMember AddMember(Guid userId, OrganizationRole role = OrganizationRole.Member)
    {
        var existing = _members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
        if (existing is not null) return existing;

        var member = OrganizationMember.Create(Guid.NewGuid(), Id, userId, role);
        _members.Add(member);
        UpdatedAt = DateTimeOffset.UtcNow;
        return member;
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
        member?.Deactivate();
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
