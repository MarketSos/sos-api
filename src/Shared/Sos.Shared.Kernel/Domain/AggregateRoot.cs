namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Корень агрегата. Реализует аудит, мягкое удаление и оптимистичную блокировку.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>, IAuditable, ISoftDeletable
{
    /// <summary>Версия агрегата для оптимистичной блокировки</summary>
    public int Version { get; protected set; }

    // --- IAuditable ---
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // --- ISoftDeletable ---
    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    public void SetCreatedBy(Guid userId) => CreatedBy = userId;

    public void SetUpdatedBy(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTimeOffset.UtcNow;
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
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
