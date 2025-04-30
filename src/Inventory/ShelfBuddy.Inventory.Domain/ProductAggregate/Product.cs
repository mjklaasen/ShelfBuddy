using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.Inventory.Domain;

public class Product(ProductCategory category, Guid? id = null) : AggregateRoot(id ?? Guid.CreateVersion7())
{
    public string Name { get; set; } = string.Empty;
    public ProductCategory Category { get; set; } = category;
}