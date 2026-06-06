using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Kernel.Domain;

namespace Sos.Core.Domain.Entities;

public class Employee : PersonBase, IHasOrganization, ISoftDeletable
{
    public Guid OrganizationId { get; set; }

    public bool            IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
    public Guid?           DeletedBy { get; protected set; }

    public Guid  UserId           { get; private set; }
    public Guid? SpecializationId { get; private set; }
    public Guid? EmployeeRankId   { get; private set; }
    public DateOnly? HireDate     { get; private set; }
    public DateOnly? FireDate     { get; private set; }
    public bool      IsActive     { get; private set; } = true;

    public virtual User?           User           { get; private set; }
    public virtual Specialization? Specialization { get; private set; }
    public virtual EmployeeRank?   EmployeeRank   { get; private set; }

    private Employee() { }

    public static Employee Create(
        Guid      userId,
        string    firstName,
        string    lastName,
        string?   middleName       = null,
        DateOnly? birthDate        = null,
        string?   phone            = null,
        Gender?   gender           = null,
        Guid?     specializationId = null,
        Guid?     employeeRankId   = null,
        DateOnly? hireDate         = null)
    {
        var e = new Employee
        {
            Id               = Guid.NewGuid(),
            UserId           = userId,
            SpecializationId = specializationId,
            EmployeeRankId   = employeeRankId,
            HireDate         = hireDate,
            IsActive         = true
        };
        e.SetPersonInfo(firstName, lastName, middleName, birthDate, phone, gender);
        return e;
    }

    public void Update(
        string    firstName,
        string    lastName,
        string?   middleName       = null,
        DateOnly? birthDate        = null,
        string?   phone            = null,
        Gender?   gender           = null,
        Guid?     specializationId = null,
        Guid?     employeeRankId   = null)
    {
        SetPersonInfo(firstName, lastName, middleName, birthDate, phone, gender);
        SpecializationId = specializationId;
        EmployeeRankId   = employeeRankId;
    }

    public void Hire(DateOnly hireDate)
    {
        HireDate = hireDate;
        FireDate = null;
        IsActive = true;
    }

    public void Fire(DateOnly fireDate)
    {
        FireDate = fireDate;
        IsActive = false;
    }

    public void SoftDelete(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
