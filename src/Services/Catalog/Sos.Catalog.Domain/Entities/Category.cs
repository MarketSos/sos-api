using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Категория товаров. Поддерживает иерархическую структуру (дерево категорий).
/// </summary>
public class Category : CatalogEntity<Guid>
{
    /// <summary>
    /// ID родительской категории (null — корневая категория)
    /// </summary>
    public Guid? ParentId { get; private set; }

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
    public static Category Create(Guid id, string nameUz, string nameRu,
        string? nameEn = null, string? nameUzKiril = null, Guid? parentId = null)
    {
        var category = new Category { Id = id, ParentId = parentId };
        category.SetNames(nameUz, nameRu, nameEn, nameUzKiril);
        return category;
    }

    /// <summary>
    /// Обновить названия категории
    /// </summary>
    public void UpdateNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzKiril = null)
        => SetNames(nameUz, nameRu, nameEn, nameUzKiril);
}
