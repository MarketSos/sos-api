namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовый класс для всех сущностей домена.
/// Содержит идентификатор, аудит и мягкое удаление.
/// </summary>
public abstract class Entity<TId> : IAuditable, ISoftDeletable
{
    /// <summary>
    /// Уникальный идентификатор сущности
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Дата создания (UTC)
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего изменения (UTC)
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// ID пользователя, создавшего запись
    /// </summary>
    public Guid? CreatedBy { get; protected set; }

    /// <summary>
    /// ID пользователя, последним изменившего запись
    /// </summary>
    public Guid? UpdatedBy { get; protected set; }

    /// <summary>
    /// Удалена ли запись (мягкое удаление)
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Дата удаления (UTC)
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// ID пользователя, удалившего запись
    /// </summary>
    public Guid? DeletedBy { get; protected set; }
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Доменные события, накопленные до сохранения
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Добавить доменное событие
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Очистить события после публикации
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Установить автора создания
    /// </summary>
    public void SetCreatedBy(Guid userId) => CreatedBy = userId;

    /// <summary>
    /// Установить автора изменения
    /// </summary>
    public void SetUpdatedBy(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Мягкое удаление — помечает запись как удалённую
    /// </summary>
    public void SoftDelete(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Восстановить мягко удалённую запись
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
