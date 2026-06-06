namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовый класс для всех сущностей домена.
/// Содержит только идентификатор и доменные события.
/// Аудит и мягкое удаление — в <see cref="AggregateRoot{TId}"/>.
/// </summary>
public abstract class Entity<TId>
{
    /// <summary>Уникальный идентификатор сущности</summary>
    public TId Id { get; set; } = default!;

    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Доменные события, накопленные до сохранения</summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Добавить доменное событие</summary>
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Очистить события после публикации</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
