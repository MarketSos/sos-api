using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

/// <summary>
/// Категория товаров. Поддерживает иерархическую структуру (дерево).
/// </summary>
public class Category : CatalogEntity<Guid>
{
    /// <summary>
    /// Идентификатор родительской категории (null — корневая)
    /// </summary>
    public Guid? ParentId { get; private set; }

    public Category?            Parent   { get; private set; }
    public ICollection<Category> Children { get; private set; } = [];
    public ICollection<Product>  Products { get; private set; } = [];

    private Category() { }

    public static Category Create(
        Guid id,
        string nameUz,
        string nameRu,
        string? nameEn      = null,
        string? nameUzCyrl = null,
        string? nameKk      = null,
        Guid? parentId      = null)
    {
        var category = new Category { Id = id, ParentId = parentId };
        category.SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
        return category;
    }

    public void UpdateNames(string nameUz, string nameRu, string? nameEn = null, string? nameUzCyrl = null, string? nameKk = null)
        => SetNames(nameUz, nameUzCyrl, nameRu, nameEn, nameKk);
}
