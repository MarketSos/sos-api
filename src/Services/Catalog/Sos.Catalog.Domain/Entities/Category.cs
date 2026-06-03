using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Категория товаров. Поддерживает иерархическую структуру (дерево категорий).
/// </summary>
public class Category : AggregateRoot<Guid>
{
    /// <summary>
    /// Название категории
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Описание категории
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// ID родительской категории (null — корневая категория)
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// Порядок сортировки в списке
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// Активна ли категория
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Родительская категория
    /// </summary>
    public Category? Parent { get; private set; }

    /// <summary>
    /// Дочерние категории
    /// </summary>
    public ICollection<Category> Children { get; private set; } = [];

    /// <summary>
    /// Товары в данной категории
    /// </summary>
    public ICollection<Product> Products { get; private set; } = [];

    private Category() { }

    /// <summary>
    /// Создать новую категорию
    /// </summary>
    public static Category Create(Guid id, string name, Guid? parentId = null)
        => new() { Id = id, Name = name, ParentId = parentId };
}
