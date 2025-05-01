using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class Product : AggregateRoot
{
    public string Name { get; set; }
    public ProductCategory ProductCategory { get; set; }

    public Product(string name, ProductCategory productCategory, Guid id) : base(id)
    {
        Name = name;
        ProductCategory = productCategory;
    }

    public Product(string name, ProductCategory productCategory) : this(name, productCategory, Guid.CreateVersion7()) { }
    public Product(string name, string categoryName) : this(name, new ProductCategory(categoryName), Guid.CreateVersion7()) { }

    private Product(string name)
    {
        Name = name;
    }
}