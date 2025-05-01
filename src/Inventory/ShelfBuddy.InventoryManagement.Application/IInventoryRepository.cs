using ErrorOr;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public interface IInventoryRepository
{
    Task<int> CreateAsync(Inventory inventory);
    Task<int> UpdateAsync(Inventory inventory);
    Task DeleteAsync(Guid inventoryId);
    Task<Inventory?> GetByIdAsync(Guid inventoryId);
    Task<IEnumerable<Inventory>> ListAsync(int page = 1, int pageSize = 10);
}