using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Repositories;

public interface ILocalProductRepository
{
    Task<LocalProduct?> GetAsync(Guid id);
    Task<List<LocalProduct>> ListAsync(int page = 1, int pageSize = 10, string? categoryName = null);
    Task<LocalProduct> AddAsync(LocalProduct product);
    Task<LocalProduct> UpdateAsync(LocalProduct product);
    Task<bool> DeleteAsync(Guid id);
    Task<List<LocalProduct>> GetUnSyncedAsync();
    Task MarkAsSyncedAsync(Guid id);
}