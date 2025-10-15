using Microsoft.EntityFrameworkCore;
using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Repositories;

public class LocalProductCategoryRepository(LocalDbContext dbContext) : ILocalProductCategoryRepository
{
    private readonly LocalDbContext _dbContext = dbContext;

    public async Task<LocalProductCategory?> GetAsync(Guid id)
    {
        return await _dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<LocalProductCategory?> GetByNameAsync(string name)
    {
        return await _dbContext.ProductCategories.FirstOrDefaultAsync(c =>
            c.Name.Equals(name, StringComparison.Ordinal) && !c.IsDeleted);
    }

    public async Task<List<LocalProductCategory>> ListAsync(int page = 1, int pageSize = 10)
    {
        return await _dbContext.ProductCategories
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<LocalProductCategory> AddAsync(LocalProductCategory category)
    {
        var existingCategory = await GetByNameAsync(category.Name);
        if (existingCategory is not null)
        {
            return existingCategory;
        }

        category.LastUpdated = DateTimeOffset.UtcNow;
        category.IsSynced = false;

        await _dbContext.ProductCategories.AddAsync(category);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding product category: {ex.Message}");
            throw;
        }

        return category;
    }

    public async Task<LocalProductCategory> UpdateAsync(LocalProductCategory category)
    {
        category.LastUpdated = DateTimeOffset.UtcNow;
        category.IsSynced = false;

        var existingEntity = await _dbContext.ProductCategories.FindAsync(category.Id);

        if (existingEntity is not null)
        {
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(category);
            category = existingEntity;
        }
        else
        {
            _dbContext.Entry(category).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _dbContext.ProductCategories.FindAsync(id);
        if (category is null)
        {
            return false;
        }

        // Check if any products are using this category
        var hasProducts = await _dbContext.Products
            .AnyAsync(p => p.ProductCategory.Id == id && !p.IsDeleted);

        if (hasProducts)
        {
            // Don't delete categories that have products
            return false;
        }

        // Soft delete
        category.IsDeleted = true;
        category.IsSynced = false;
        category.LastUpdated = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<LocalProductCategory>> GetUnSyncedAsync()
    {
        return await _dbContext.ProductCategories
            .Where(c => !c.IsSynced)
            .ToListAsync();
    }

    public async Task MarkAsSyncedAsync(Guid id)
    {
        var category = await _dbContext.ProductCategories.FindAsync(id);
        if (category is not null)
        {
            category.IsSynced = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
