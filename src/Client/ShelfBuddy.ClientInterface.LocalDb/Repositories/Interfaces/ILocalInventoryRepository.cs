using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Repositories;

public interface ILocalInventoryRepository
{
    Task<LocalInventory?> GetAsync(Guid id);
    Task<List<LocalInventory>> ListAsync(Guid userId, int page = 1, int pageSize = 10);
    Task<LocalInventory> AddAsync(LocalInventory inventory);
    Task<LocalInventory> UpdateAsync(LocalInventory inventory);
    Task<bool> DeleteAsync(Guid id);
    Task<List<LocalInventory>> GetUnSyncedAsync();
    Task MarkAsSyncedAsync(Guid id);
}