using ErrorOr;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.ClientInterface.Services;

public interface IInventoryService
{
    Task<ErrorOr<InventoryDto>> GetAsync(Guid id);
    Task<List<InventoryDto>> ListAsync(Guid userId, int page = 1, int pageSize = 10);
    Task<ErrorOr<InventoryDto>> CreateAsync(string name, Guid userId);
    Task<ErrorOr<Updated>> UpdateAsync(InventoryDto inventory);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid inventoryId);
}