using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class Product(string name, ProductCategory category, Guid id) : AggregateRoot(id)
{
    public Product(string name, ProductCategory category) : this(name, category, Guid.CreateVersion7()) { }

    public string Name { get; set; } = name;
    public ProductCategory Category { get; set; } = category;

    private Product() : this("", new ProductCategory()) { }
}