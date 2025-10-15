using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Repositories;

public interface ILocalProductCategoryRepository
{
    Task<LocalProductCategory?> GetAsync(Guid id);
    Task<LocalProductCategory?> GetByNameAsync(string name);
    Task<List<LocalProductCategory>> ListAsync(int page = 1, int pageSize = 10);
    Task<LocalProductCategory> AddAsync(LocalProductCategory category);
    Task<LocalProductCategory> UpdateAsync(LocalProductCategory category);
    Task<bool> DeleteAsync(Guid id);
    Task<List<LocalProductCategory>> GetUnSyncedAsync();
    Task MarkAsSyncedAsync(Guid id);
}