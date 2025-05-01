using Microsoft.EntityFrameworkCore;
using ShelfBuddy.InventoryManagement.Application;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Infrastructure.Persistence;

public class ProductRepository(InventoryDbContext dbContext) : IProductRepository
{
    private readonly InventoryDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(Product product)
    {
        var productCategory = _dbContext.ProductCategories.AsEnumerable()
            .FirstOrDefault(x => x.Name.Equals(product.ProductCategory.Name, StringComparison.OrdinalIgnoreCase));
        if (productCategory is null)
        {
            _dbContext.ProductCategories.Add(product.ProductCategory);
            productCategory = product.ProductCategory;
        }

        product.UpdateProductCategory(productCategory);
        await _dbContext.Products.AddAsync(product);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Product product)
    {
        _dbContext.Products.Update(product);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product is null)
        {
            return;
        }
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Products.Include(product => product.ProductCategory)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Product>> ListAsync(int page = 1, int pageSize = 10)
    {
        return await _dbContext.Products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}