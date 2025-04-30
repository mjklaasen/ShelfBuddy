using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class ProductCategory : Entity
{
    internal ProductCategory(Guid? id = null) : base(id ?? Guid.CreateVersion7()) { }

    public string Name { get; internal set; } = string.Empty;
}