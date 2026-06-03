namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Мягкое удаление — запись помечается как удалённая, не удаляется физически.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>Удалена ли запись</summary>
    bool IsDeleted { get; }

    /// <summary>Дата удаления (UTC)</summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>ID пользователя, удалившего запись</summary>
    Guid? DeletedBy { get; }
}
