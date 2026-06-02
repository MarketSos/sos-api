using Sos.Shared.Kernel.Domain;

namespace Sos.Catalog.Domain.Entities;

public class Category : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Category? Parent { get; private set; }
    public ICollection<Category> Children { get; private set; } = [];
    public ICollection<Product> Products { get; private set; } = [];

    private Category() { }

    public static Category Create(Guid id, string name, Guid? parentId = null)
        => new() { Id = id, Name = name, ParentId = parentId };
}
