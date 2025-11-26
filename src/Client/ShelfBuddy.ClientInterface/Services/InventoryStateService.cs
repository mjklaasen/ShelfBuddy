using ErrorOr;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.ClientInterface.Services;

internal sealed class InventoryStateService(IInventoryService inventoryService, IPreferences preferences) : IInventoryStateService
{
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly IPreferences _preferences = preferences;
    public IList<InventoryDto> UserInventories => _userInventories;
    private List<InventoryDto> _userInventories = [];
    public event EventHandler? OnInventoryChanged;
    public event EventHandler? OnInventoryListChanged;
    public event EventHandler? InventoryPageRefreshRequested;
    public InventoryDto? CurrentInventory { get; private set; }
    public bool HasActiveInventory => CurrentInventory is not null;
    public bool IsError { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsLoading { get; private set; }

    public async Task InitializeAsync(Guid userId)
    {
        IsInitialized = false;
        IsLoading = true;

        if (Guid.TryParse(_preferences.Get("CurrentInventoryId", Guid.Empty.ToString()), out var lastInventoryId) &&
            lastInventoryId != Guid.Empty)
        {
            await SetCurrentInventoryAsync(lastInventoryId);
            await LoadUserInventoriesAsync(userId);
            IsInitialized = true;
            IsLoading = false;
            return;
        }

        await LoadUserInventoriesAsync(userId);
        if (_userInventories.Count > 0 && CurrentInventory is null)
        {
            await SetCurrentInventoryAsync(_userInventories[0].Id);
        }
        IsInitialized = true;
        IsLoading = false;
    }

    public async Task SetCurrentInventoryAsync(Guid? inventoryId)
    {
        if (inventoryId is null)
        {
            CurrentInventory = null;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
            _preferences.Remove("CurrentInventoryId");
            return;
        }
        
        var inventory = _userInventories.FirstOrDefault(i => i.Id == inventoryId);
        if (inventory is not null)
        {
            CurrentInventory = inventory;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
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
                OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                _preferences.Set("CurrentInventoryId", inventoryId.Value.ToString());
                return;
            }

            CurrentInventory = null;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            // Handle errors
            CurrentInventory = null;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task LoadDefaultInventoryAsync(Guid userId)
    {
        try
        {
            await LoadUserInventoriesAsync(userId);

            if (_userInventories.Count > 0)
            {
                await SetCurrentInventoryAsync(_userInventories[0].Id);
            }
        }
        catch
        {
            // Handle errors
            CurrentInventory = null;
            OnInventoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task LoadUserInventoriesAsync(Guid userId)
    {
        try
        {
            var inventories = await _inventoryService.ListAsync(userId);

            if (inventories.Count > 0)
            {
                _userInventories = inventories;
                IsError = false;
            }

            OnInventoryListChanged?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            _userInventories = [];
            IsError = true;
            OnInventoryListChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public async Task RefreshInventoriesAsync(Guid userId)
    {
        try
        {
            var inventories = await _inventoryService.ListAsync(userId);

            if (inventories.Count > 0)
            {
                _userInventories = inventories;
                if (CurrentInventory is not null)
                {
                    await SetCurrentInventoryAsync(CurrentInventory.Id);
                }

                IsError = false;
                OnInventoryListChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch
        {
            _userInventories = [];
            IsError = true;
            OnInventoryListChanged?.Invoke(this, EventArgs.Empty);
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
                _userInventories.RemoveAll(i => i.Id == inventoryId);
                return Result.Deleted;
            }

            var currentInventoryIndex = _userInventories.Index().FirstOrDefault(x => x.Item.Id.Equals(CurrentInventory.Id)).Index;

            var nextIndex = currentInventoryIndex - 1;
            if (nextIndex >= 0)
            {
                await SetCurrentInventoryAsync(_userInventories[nextIndex].Id);
                _userInventories.RemoveAll(i => i.Id == inventoryId);
                return Result.Deleted;
            }

            _userInventories.RemoveAll(i => i.Id == inventoryId);
            await SetCurrentInventoryAsync(_userInventories.FirstOrDefault()?.Id);
        }
        catch
        {
            // Handle errors - optionally log them
        }

        return Result.Deleted;
    }

    public void NotifyInventoryPageRefreshRequested()
    {
        InventoryPageRefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
