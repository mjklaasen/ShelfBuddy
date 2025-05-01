using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public interface IProductRepository
{
    Task<int> CreateAsync(Product product);
    Task<int> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> ListAsync(int page = 1, int pageSize = 10);
}