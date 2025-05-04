using ShelfBuddy.Contracts;
using System.Net.Http.Json;
using ErrorOr;

namespace ShelfBuddy.ClientInterface.Services;

public class InventoryStateService(IHttpClientFactory httpClientFactory, IPreferences preferences) : IInventoryStateService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IPreferences _preferences = preferences;
    public List<InventoryDto> UserInventories { get; private set; } = [];
    public event Action? OnInventoryChanged;
    public event Func<Task>? InventoryPageRefreshRequested;
    public InventoryDto? CurrentInventory { get; private set; }
    public bool HasActiveInventory => CurrentInventory is not null;

    public async Task InitializeAsync(Guid userId)
    {
        var lastInventoryId = Guid.Parse(_preferences.Get("CurrentInventoryId", Guid.Empty.ToString()));
        if (lastInventoryId != Guid.Empty)
        {
            await SetCurrentInventoryAsync(lastInventoryId);
            await LoadUserInventoriesAsync(userId);
            return;
        }

        await LoadUserInventoriesAsync(userId);
        if (UserInventories.Count > 0 && CurrentInventory is null)
        {
            await SetCurrentInventoryAsync(UserInventories[0].Id);
        }
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
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetAsync(new Uri($"/inventory/{inventoryId}", UriKind.Relative));

            if (response.IsSuccessStatusCode)
            {
                CurrentInventory = await response.Content.ReadFromJsonAsync<InventoryDto>();
                OnInventoryChanged?.Invoke();
                _preferences.Set("CurrentInventoryId", inventoryId.Value.ToString());
            }
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
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetAsync(new Uri($"/inventory?userId={userId}", UriKind.Relative));

            if (response.IsSuccessStatusCode)
            {
                UserInventories = await response.Content.ReadFromJsonAsync<List<InventoryDto>>() ?? [];
            }
        }
        catch
        {
            // Handle errors
        }
    }

    public async Task RefreshInventoriesAsync(Guid userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.GetAsync(new Uri($"/inventory?userId={userId}", UriKind.Relative));

            if (response.IsSuccessStatusCode)
            {
                UserInventories = await response.Content.ReadFromJsonAsync<List<InventoryDto>>() ?? [];
                
                if (CurrentInventory is not null)
                {
                    await SetCurrentInventoryAsync(CurrentInventory.Id);
                }
            }
        }
        catch
        {
            // Handle errors - optionally log them
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteInventoryAsync(Guid inventoryId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("api");
            var response = await client.DeleteAsync(new Uri($"/inventory/{inventoryId}", UriKind.Relative));
            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure(code: "InventoryStateService.DeleteInventoryFailure", description: response.ReasonPhrase ?? "Failure to delete inventory.");
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