namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовый класс для всех сущностей домена.
/// Содержит идентификатор, дату создания/обновления и список доменных событий.
/// </summary>
public abstract class Entity<TId>
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
}
