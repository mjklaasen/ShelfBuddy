using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class ProductCategory : Entity
{
    internal ProductCategory(string name, Guid id) : base(id)
    {
        Name = name;
    }

    internal ProductCategory(string name) : this(name, Guid.CreateVersion7()) { }

    public string Name { get; internal set; }
}