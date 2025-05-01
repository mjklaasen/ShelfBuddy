using Microsoft.EntityFrameworkCore;
using ShelfBuddy.InventoryManagement.Application;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Infrastructure.Persistence;

public class InventoryRepository(InventoryDbContext dbContext) : IInventoryRepository
{
    private readonly InventoryDbContext _dbContext = dbContext;

    public async Task<int> CreateAsync(Inventory inventory)
    {
        await _dbContext.Inventories.AddAsync(inventory);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Inventory inventory)
    {
        _dbContext.Inventories.Update(inventory);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var inventory = await _dbContext.Inventories.FindAsync(id);
        if (inventory is null)
        {
            return;
        }
        _dbContext.Inventories.Remove(inventory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Inventory?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Inventories.FindAsync(id);
    }

    public async Task<IEnumerable<Inventory>> ListAsync(int page = 1, int pageSize = 10, Guid? userId = null)
    {
        if (userId is null)
        {
            return await _dbContext.Inventories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        return await _dbContext.Inventories
            .Where(inventory => inventory.UserId == userId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}