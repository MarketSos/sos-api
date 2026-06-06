namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Ko'p tillli nomlarga ega reference/katalog entitylar uchun baza sinf.
/// AggregateRoot emas — audit maydonlari yo'q, faqat tenant scope va soft-delete.
/// </summary>
public abstract class LocalizableEntity<TId> : Entity<TId>, ISoftDeletable
{
    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    public string NameUz { get; protected set; } = default!;
    public string NameRu { get; protected set; } = default!;
    public string? NameEn { get; protected set; }
    public string? NameUzKiril { get; protected set; }

    public string GetName(string lang = "uz") => lang switch
    {
        "ru"       => NameRu,
        "en"       => NameEn ?? NameUz,
        "uz-kiril" => NameUzKiril ?? NameUz,
        _          => NameUz
    };

    protected void SetNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzKiril = null)
    {
        NameUz      = nameUz;
        NameRu      = nameRu;
        NameEn      = nameEn;
        NameUzKiril = nameUzKiril;
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
