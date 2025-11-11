using ErrorOr;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.ClientInterface.Services;

internal interface IInventoryStateService
{
    event EventHandler? OnInventoryChanged;
    event EventHandler? OnInventoryListChanged;
    event EventHandler? InventoryPageRefreshRequested;
    InventoryDto? CurrentInventory { get; }
    IList<InventoryDto> UserInventories { get; }
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
    void NotifyInventoryPageRefreshRequested();
}
