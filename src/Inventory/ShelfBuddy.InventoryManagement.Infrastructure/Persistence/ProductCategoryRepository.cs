using Microsoft.EntityFrameworkCore;
using ShelfBuddy.InventoryManagement.Application;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Infrastructure.Persistence;

public class ProductCategoryRepository(InventoryDbContext dbContext) : IProductCategoryRepository
{
    private readonly InventoryDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(ProductCategory productCategory)
    {
        await _dbContext.ProductCategories.AddAsync(productCategory);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(ProductCategory productCategory)
    {
        _dbContext.ProductCategories.Update(productCategory);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var productCategory = await _dbContext.ProductCategories.FindAsync(id);
        if (productCategory is null)
        {
            return;
        }
        _dbContext.ProductCategories.Remove(productCategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ProductCategory?> GetByIdAsync(Guid id)
    {
        return await _dbContext.ProductCategories.FindAsync(id);
    }

    public async Task<ProductCategory?> GetByNameAsync(string name)
    {
        return await _dbContext.ProductCategories
            .FirstOrDefaultAsync(pc => pc.Name == name);
    }

    public async Task<IEnumerable<ProductCategory>> ListAsync(int page = 1, int pageSize = 10)
    {
        return await _dbContext.ProductCategories
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}