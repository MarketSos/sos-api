namespace Sos.Shared.Kernel.Domain;

public abstract class PersonBase : Entity<Guid>
{
    public string    FirstName  { get; protected set; } = default!;
    public string    LastName   { get; protected set; } = default!;
    public string?   MiddleName { get; protected set; }
    public DateOnly? BirthDate  { get; protected set; }
    public string?   Phone      { get; protected set; }
    public Gender?   Gender     { get; protected set; }

    public string FullName =>
        MiddleName is not null
            ? $"{LastName} {FirstName} {MiddleName}"
            : $"{LastName} {FirstName}";

    protected void SetPersonInfo(
        string    firstName,
        string    lastName,
        string?   middleName = null,
        DateOnly? birthDate  = null,
        string?   phone      = null,
        Gender?   gender     = null)
    {
        FirstName  = firstName.Trim();
        LastName   = lastName.Trim();
        MiddleName = middleName?.Trim();
        BirthDate  = birthDate;
        Phone      = phone?.Trim();
        Gender     = gender;
    }
}
