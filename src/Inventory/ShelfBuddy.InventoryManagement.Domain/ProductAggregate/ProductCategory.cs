using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class ProductCategory : Entity
{
    internal ProductCategory(Guid id) : base(id) { }
    internal ProductCategory() : this(Guid.CreateVersion7()) { }

    public string Name { get; internal set; } = string.Empty;
}