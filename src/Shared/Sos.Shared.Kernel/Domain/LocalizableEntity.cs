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
    public string NameUzCyrl { get; protected set; } = default!;
    public string NameRu { get; protected set; } = default!;
    public string? NameEn { get; protected set; }

    /// <summary>Qoraqalpog'on tili (Qaraqalpaq)</summary>
    public string? NameKk { get; protected set; }

    public string GetName(string lang = "uz") => lang switch
    {
        "ru"      => NameRu,
        "en"      => NameEn ?? NameUz,
        "uz-cyrl" => NameUzCyrl,
        "kk"      => NameKk ?? NameUz,
        _         => NameUz
    };

    protected void SetNames(
        string nameUz,
        string nameUzCyrl,
        string nameRu,
        string? nameEn = null,
        string? nameKk = null)
    {
        NameUz     = nameUz;
        NameUzCyrl = nameUzCyrl;
        NameRu     = nameRu;
        NameEn     = nameEn;
        NameKk     = nameKk;
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
