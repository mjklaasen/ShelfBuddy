using Microsoft.EntityFrameworkCore;
using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Repositories;

public class LocalProductRepository(LocalDbContext dbContext) : ILocalProductRepository
{
    private readonly LocalDbContext _dbContext = dbContext;

    public async Task<LocalProduct?> GetAsync(Guid id)
    {
        return await _dbContext.Products
            .Include(p => p.ProductCategory)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<List<LocalProduct>> ListAsync(int page = 1, int pageSize = 10, string? categoryName = null)
    {
        var query = _dbContext.Products
            .Include(p => p.ProductCategory)
            .Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(p => p.ProductCategory.Name.Equals(categoryName, StringComparison.Ordinal));
        }

        return await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<LocalProduct> AddAsync(LocalProduct product)
    {
        product.LastUpdated = DateTimeOffset.UtcNow;
        product.IsSynced = false;

        if (product.ProductCategory.Id == Guid.Empty)
        {
            var existingCategory = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(c => c.Name == product.ProductCategory.Name);

            if (existingCategory is not null)
            {
                product.ProductCategory = existingCategory;
            }
        }

        await _dbContext.Products.AddAsync(product);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding product: {ex.Message}");
            throw;
        }

        return product;
    }

    public async Task<LocalProduct> UpdateAsync(LocalProduct product)
    {
        product.LastUpdated = DateTimeOffset.UtcNow;
        product.IsSynced = false;

        var existingEntity = await _dbContext.Products.FindAsync(product.Id);
        if (existingEntity is not null)
        {
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(product);

            var existingCategory = await _dbContext.ProductCategories
                .FindAsync(product.ProductCategory.Id);

            if (existingCategory is null)
            {
                await _dbContext.ProductCategories.AddAsync(product.ProductCategory);
            }
            else
            {
                existingEntity.ProductCategory = existingCategory;
            }

            await _dbContext.SaveChangesAsync();
            return product;
        }

        _dbContext.Entry(product).State = EntityState.Modified;

        var category = await _dbContext.ProductCategories
            .FindAsync(product.ProductCategory.Id);

        if (category is null)
        {
            _dbContext.Entry(product.ProductCategory).State = EntityState.Added;
        }
        else
        {
            product.ProductCategory = category;
        }

        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product is null)
        {
            return false;
        }

        // Soft delete
        product.IsDeleted = true;
        product.IsSynced = false;
        product.LastUpdated = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<LocalProduct>> GetUnSyncedAsync()
    {
        return await _dbContext.Products
            .Include(p => p.ProductCategory)
            .Where(p => !p.IsSynced)
            .ToListAsync();
    }

    public async Task MarkAsSyncedAsync(Guid id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product is not null)
        {
            product.IsSynced = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
