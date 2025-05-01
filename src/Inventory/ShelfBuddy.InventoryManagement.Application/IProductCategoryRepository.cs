using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public interface IProductCategoryRepository
{
    Task<int> CreateAsync(ProductCategory productCategory);
    Task<int> UpdateAsync(ProductCategory productCategory);
    Task DeleteAsync(Guid id);
    Task<ProductCategory?> GetByIdAsync(Guid id);
    Task<ProductCategory?> GetByNameAsync(string name);
    Task<IEnumerable<ProductCategory>> ListAsync(int page = 1, int pageSize = 10);
}