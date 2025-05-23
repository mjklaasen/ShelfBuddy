﻿using ShelfBuddy.SharedKernel;

namespace ShelfBuddy.InventoryManagement.Domain;

public class Inventory(string name, Guid userId, Guid id) : AggregateRoot(id)
{
    private readonly Dictionary<Guid, int> _products = [];
    public string Name { get; set; } = name;
    public Guid UserId { get; set; } = userId;
    public IReadOnlyDictionary<Guid, int> Products => _products;

    public Inventory(string name, Guid userId) : this(name, userId, Guid.CreateVersion7()) { }

    public void AddProduct(Guid productId, int quantity)
    {
        if (!_products.TryAdd(productId, quantity))
        {
            _products[productId] += quantity;
        }
    }

    public void RemoveProduct(Guid productId, int quantity)
    {
        if (!_products.TryGetValue(productId, out var currentQuantity))
        {
            return;
        }

        _products[productId] -= quantity;

        if (currentQuantity <= quantity)
        {
            _products.Remove(productId);
        }
    }

    public int GetProductQuantity(Guid productId)
    {
        return _products.GetValueOrDefault(productId, 0);
    }
}