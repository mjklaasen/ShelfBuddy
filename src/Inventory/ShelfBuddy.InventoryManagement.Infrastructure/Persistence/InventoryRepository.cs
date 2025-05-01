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

    public async Task DeleteAsync(Guid inventoryId)
    {
        var inventory = await _dbContext.Inventories.FindAsync(inventoryId);
        if (inventory is null)
        {
            return;
        }
        _dbContext.Inventories.Remove(inventory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Inventory?> GetByIdAsync(Guid inventoryId)
    {
        return await _dbContext.Inventories.FindAsync(inventoryId);
    }

    public async Task<IEnumerable<Inventory>> ListAsync(int page = 1, int pageSize = 10)
    {
        return await _dbContext.Inventories
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}