namespace Sos.Shared.Kernel.Domain;

/// <summary>
/// Базовый класс для каталожных сущностей.
/// Многоязычное название, описание, активность и сортировка.
/// </summary>
public abstract class CatalogEntity<TId> : DescribedEntity<TId>
{
    /// <summary>
    /// Активна ли запись в каталоге
    /// </summary>
    public bool IsActive { get; protected set; } = true;

    /// <summary>
    /// Порядок сортировки в списке
    /// </summary>
    public int SortOrder { get; protected set; }

    /// <summary>
    /// Деактивировать запись
    /// </summary>
    public virtual void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }

    /// <summary>
    /// Активировать запись
    /// </summary>
    public virtual void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }

    /// <summary>
    /// Изменить порядок сортировки
    /// </summary>
    public void SetSortOrder(int order) { SortOrder = order; UpdatedAt = DateTime.UtcNow; }
}
