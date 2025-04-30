using ErrorOr;
using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class Inventory(string name, Guid userId, Guid? id = null) : AggregateRoot(id ?? Guid.CreateVersion7())
{
    private readonly Dictionary<Guid, int> _products = [];
    public string Name { get; set; } = name;
    public Guid UserId { get; } = userId;

    public void AddProduct(Guid productId, int quantity)
    {
        if (!_products.TryAdd(productId, quantity))
        {
            _products[productId] += quantity;
        }
    }

    public ErrorOr<Success> RemoveProduct(Guid productId, int quantity)
    {
        if (!_products.TryGetValue(productId, out var currentQuantity))
        {
            return Error.NotFound(code: "Inventory.ProductNotFound",
                description: "This inventory does not contain this product.");
        }

        _products[productId] -= quantity;

        if (currentQuantity <= quantity)
        {
            _products.Remove(productId);
        }
        
        return Result.Success;
    }

    public int GetProductQuantity(Guid productId)
    {
        return _products.GetValueOrDefault(productId, 0);
    }
}