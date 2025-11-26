using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class Product : AggregateRoot
{
    public string Name { get; set; }
    public ProductCategory ProductCategory { get; private set; }

    public Product(string name, ProductCategory productCategory, Guid id) : base(id)
    {
        Name = name;
        ProductCategory = productCategory;
    }

    public Product(string name, ProductCategory productCategory) : this(name, productCategory, Guid.CreateVersion7()) { }
    public Product(string name, string categoryName) : this(name, new ProductCategory(categoryName), Guid.CreateVersion7()) { }
    // TODO: find a fix for Copilot comment below:
    // Adding a hardcoded default ProductCategory("Uncategorized") in the private constructor creates an implicit
    // coupling and inconsistency. If this constructor is meant for EF Core, consider documenting it or making the category nullable.
    // If "Uncategorized" is a business rule, consider defining it as a constant or making it explicit in the public API.
    private Product(string name)
    {
        Name = name;
        ProductCategory = new ProductCategory("Uncategorized");
    }

    public void UpdateProductCategory(ProductCategory productCategory)
    {
        ProductCategory = productCategory;
    }

    public void UpdateProductCategory(string productCategory)
    {
        ProductCategory = new ProductCategory(productCategory);
    }
}
