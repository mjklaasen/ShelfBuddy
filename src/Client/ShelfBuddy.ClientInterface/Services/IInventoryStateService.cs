using ErrorOr;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.ClientInterface.Services;

public interface IInventoryStateService
{
    event Action? OnInventoryChanged;
    event Action? OnInventoryListChanged;
    event Func<Task>? InventoryPageRefreshRequested;
    InventoryDto? CurrentInventory { get; }
    List<InventoryDto> UserInventories { get; }
    bool HasActiveInventory { get; }
    bool IsError { get; }
    bool IsInitialized { get; }
    bool IsLoading { get; }
    Task SetCurrentInventoryAsync(Guid? inventoryId);
    Task InitializeAsync(Guid userId);
    Task LoadDefaultInventoryAsync(Guid userId);
    Task LoadUserInventoriesAsync(Guid userId);
    Task RefreshInventoriesAsync(Guid userId);
    Task<ErrorOr<Deleted>> DeleteInventoryAsync(Guid inventoryId);
    Task NotifyInventoryPageRefreshRequestedAsync();
}
