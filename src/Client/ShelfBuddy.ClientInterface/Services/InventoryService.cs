using ErrorOr;
using ShelfBuddy.Contracts;
using System.Net.Http.Json;

namespace ShelfBuddy.ClientInterface.Services;

public class InventoryService(IHttpClientFactory httpClientFactory) : EntityServiceBase, IInventoryService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<ErrorOr<InventoryDto>> GetAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.GetAsync(new Uri($"/api/v1/inventories/{id}", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        var inventory = await response.Content.ReadFromJsonAsync<InventoryDto>();
        if (inventory is null)
        {
            return Error.Failure(description: "Failed to get inventory: response invalid.");
        }

        return inventory;
    }

    public async Task<List<InventoryDto>> ListAsync(Guid userId, int page = 1, int pageSize = 10)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response =
            await client.GetAsync(new Uri($"/api/v1/inventories?userId={userId}&page={page}&pageSize={pageSize}",
                UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<InventoryDto>>() ?? [];
        }

        return [];
    }

    public async Task<ErrorOr<InventoryDto>> CreateAsync(string name, Guid userId)
    {
        var client = _httpClientFactory.CreateClient("api");

        var createRequest = new
        {
            Name = name,
            UserId = userId
        };

        var response = await client.PostAsJsonAsync("/api/v1/inventories", createRequest);
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        var created = await response.Content.ReadFromJsonAsync<InventoryDto>();
        if (created is null)
        {
            return Error.Failure(description: "Failed to create inventory: response invalid.");
        }

        return created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(InventoryDto inventory)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.PutAsJsonAsync(new Uri($"/api/v1/inventories/{inventory.Id}", UriKind.Relative), inventory);
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid inventoryId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.DeleteAsync(new Uri($"/api/v1/inventories/{inventoryId}", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        return Result.Deleted;
    }
}