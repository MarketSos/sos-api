namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Интерфейс мягкого удаления.
/// Запись не удаляется физически — только помечается как удалённая.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Удалена ли запись
    /// </summary>
    public bool IsDeleted { get; }

    /// <summary>
    /// Дата удаления (UTC)
    /// </summary>
    public DateTime? DeletedAt { get; }

    /// <summary>
    /// ID пользователя, удалившего запись
    /// </summary>
    public Guid? DeletedBy { get; }
}
