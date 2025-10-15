using Microsoft.EntityFrameworkCore;
using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Repositories;

public class LocalInventoryRepository(LocalDbContext dbContext) : ILocalInventoryRepository
{
    private readonly LocalDbContext _dbContext = dbContext;

    public async Task<LocalInventory?> GetAsync(Guid id)
    {
        return await _dbContext.Inventories.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<List<LocalInventory>> ListAsync(Guid userId, int page = 1, int pageSize = 10)
    {
        return await _dbContext.Inventories
            .Where(i => i.UserId == userId && !i.IsDeleted)
            .OrderBy(i => i.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<LocalInventory> AddAsync(LocalInventory inventory)
    {
        inventory.LastUpdated = DateTimeOffset.UtcNow;
        inventory.IsSynced = false;

        await _dbContext.Inventories.AddAsync(inventory);
        await _dbContext.SaveChangesAsync();

        return inventory;
    }

    public async Task<LocalInventory> UpdateAsync(LocalInventory inventory)
    {
        inventory.LastUpdated = DateTimeOffset.UtcNow;
        inventory.IsSynced = false;

        var existingEntity = await _dbContext.Inventories.FindAsync(inventory.Id);


        if (existingEntity is not null)
        {
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(inventory);
            inventory = existingEntity;
        }
        else
        {
            _dbContext.Entry(inventory).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync();

        return inventory;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var inventory = await _dbContext.Inventories.FindAsync(id);
        if (inventory == null)
        {
            return false;
        }

        // Soft delete
        inventory.IsDeleted = true;
        inventory.IsSynced = false;
        inventory.LastUpdated = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<LocalInventory>> GetUnSyncedAsync()
    {
        return await _dbContext.Inventories
            .Where(i => !i.IsSynced)
            .ToListAsync();
    }

    public async Task MarkAsSyncedAsync(Guid id)
    {
        var inventory = await _dbContext.Inventories.FindAsync(id);
        if (inventory is not null)
        {
            inventory.IsSynced = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}