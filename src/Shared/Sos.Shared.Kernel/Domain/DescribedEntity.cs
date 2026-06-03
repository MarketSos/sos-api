namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовый класс для сущностей с многоязычным названием и описанием.
/// </summary>
public abstract class DescribedEntity<TId> : NamedEntity<TId>
{
    /// <summary>
    /// Описание сущности (необязательно)
    /// </summary>
    public string? Description { get; protected set; }
}
