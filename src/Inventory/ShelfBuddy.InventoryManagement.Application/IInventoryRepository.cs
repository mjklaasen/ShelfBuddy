using ErrorOr;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public interface IInventoryRepository
{
    Task<ErrorOr<Created>> CreateAsync(Inventory inventory);
    Task<ErrorOr<Updated>> UpdateAsync(Inventory inventory);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid inventoryId);
    Task<ErrorOr<Inventory>> GetByIdAsync(Guid inventoryId);
}