using ErrorOr;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.ClientInterface.Services;

public class InventoryStateService(IInventoryService inventoryService, IPreferences preferences) : IInventoryStateService
{
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly IPreferences _preferences = preferences;
    public List<InventoryDto> UserInventories { get; private set; } = [];
    public event Action? OnInventoryChanged;
    public event Action? OnInventoryListChanged;
    public event Func<Task>? InventoryPageRefreshRequested;
    public InventoryDto? CurrentInventory { get; private set; }
    public bool HasActiveInventory => CurrentInventory is not null;
    public bool IsError { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsLoading { get; private set; }

    public async Task InitializeAsync(Guid userId)
    {
        IsInitialized = false;
        IsLoading = true;
        var lastInventoryId = Guid.Parse(_preferences.Get("CurrentInventoryId", Guid.Empty.ToString()));
        if (lastInventoryId != Guid.Empty)
        {
            await SetCurrentInventoryAsync(lastInventoryId);
            await LoadUserInventoriesAsync(userId);
            IsInitialized = true;
            return;
        }

        await LoadUserInventoriesAsync(userId);
        if (UserInventories.Count > 0 && CurrentInventory is null)
        {
            await SetCurrentInventoryAsync(UserInventories[0].Id);
        }
        IsInitialized = true;
        IsLoading = false;
    }

    public async Task SetCurrentInventoryAsync(Guid? inventoryId)
    {
        if (inventoryId is null)
        {
            CurrentInventory = null;
            OnInventoryChanged?.Invoke();
            _preferences.Remove("CurrentInventoryId");
            return;
        }
        
        var inventory = UserInventories.FirstOrDefault(i => i.Id == inventoryId);
        if (inventory is not null)
        {
            CurrentInventory = inventory;
            OnInventoryChanged?.Invoke();
            _preferences.Set("CurrentInventoryId", inventoryId.Value.ToString());
            return;
        }

        try
        {
            var inv = await _inventoryService.GetAsync(inventoryId.Value);
            if (!inv.IsError)
            {
                CurrentInventory = inv.Value;
                IsError = false;
                OnInventoryChanged?.Invoke();
                _preferences.Set("CurrentInventoryId", inventoryId.Value.ToString());
            }

            CurrentInventory = null;
            OnInventoryChanged?.Invoke();
        }
        catch
        {
            // Handle errors
            CurrentInventory = null;
            OnInventoryChanged?.Invoke();
        }
    }

    public async Task LoadDefaultInventoryAsync(Guid userId)
    {
        try
        {
            await LoadUserInventoriesAsync(userId);

            if (UserInventories.Count > 0)
            {
                await SetCurrentInventoryAsync(UserInventories[0].Id);
            }
        }
        catch
        {
            // Handle errors
            CurrentInventory = null;
            OnInventoryChanged?.Invoke();
        }
    }

    public async Task LoadUserInventoriesAsync(Guid userId)
    {
        try
        {
            var inventories = await _inventoryService.ListAsync(userId);

            if (inventories.Count > 0)
            {
                UserInventories = inventories;
                IsError = false;
            }

            OnInventoryListChanged?.Invoke();
        }
        catch
        {
            UserInventories = [];
            IsError = true;
            OnInventoryListChanged?.Invoke();
        }
    }

    public async Task RefreshInventoriesAsync(Guid userId)
    {
        try
        {
            var inventories = await _inventoryService.ListAsync(userId);

            if (inventories.Count > 0)
            {
                UserInventories = inventories;
                if (CurrentInventory is not null)
                {
                    await SetCurrentInventoryAsync(CurrentInventory.Id);
                }

                IsError = false;
                OnInventoryListChanged?.Invoke();
            }
        }
        catch
        {
            UserInventories = [];
            IsError = true;
            OnInventoryListChanged?.Invoke();
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteInventoryAsync(Guid inventoryId)
    {
        try
        {
            var deleteInventoryResponse = await _inventoryService.DeleteAsync(inventoryId);
            if (deleteInventoryResponse.IsError)
            {
                return deleteInventoryResponse;
            }

            if (CurrentInventory?.Id != inventoryId)
            {
                UserInventories.RemoveAll(i => i.Id == inventoryId);
                return Result.Deleted;
            }

            var currentInventoryIndex = UserInventories.Index().FirstOrDefault(x => x.Item.Id.Equals(CurrentInventory.Id)).Index;

            var nextIndex = currentInventoryIndex - 1;
            if (nextIndex >= 0)
            {
                await SetCurrentInventoryAsync(UserInventories[nextIndex].Id);
                UserInventories.RemoveAll(i => i.Id == inventoryId);
                return Result.Deleted;
            }

            UserInventories.RemoveAll(i => i.Id == inventoryId);
            await SetCurrentInventoryAsync(UserInventories.FirstOrDefault()?.Id);
        }
        catch
        {
            // Handle errors - optionally log them
        }

        return Result.Deleted;
    }

    public async Task NotifyInventoryPageRefreshRequestedAsync()
    {
        if (InventoryPageRefreshRequested is not null)
        {
            await InventoryPageRefreshRequested.Invoke();
        }
    }
}