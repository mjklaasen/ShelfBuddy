using ErrorOr;
using ShelfBuddy.Contracts;
using System.Net.Http.Json;

namespace ShelfBuddy.ClientInterface.Services;

internal class ProductCategoryService(IHttpClientFactory httpClientFactory) : EntityServiceBase, IProductCategoryService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<ErrorOr<ProductCategoryDto>> GetByIdAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.GetAsync(new Uri($"/api/v1/productCategories/{id}", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        var category = await response.Content.ReadFromJsonAsync<ProductCategoryDto>();
        if (category is null)
        {
            return Error.Failure(description: "Failed to get product category: response invalid.");
        }

        return category;
    }

    public async Task<ErrorOr<ProductCategoryDto>> GetByNameAsync(string name)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.GetAsync(new Uri($"/api/v1/productCategories/{name}", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }
        var category = await response.Content.ReadFromJsonAsync<ProductCategoryDto>();
        if (category is null)
        {
            return Error.Failure(description: "Failed to get product category: response invalid.");
        }
        return category;
    }

    public async Task<List<ProductCategoryDto>> ListAsync(int page = 1, int pageSize = 10)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.GetAsync(new Uri($"/api/v1/productCategories?page={page}&pageSize={pageSize}", UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ProductCategoryDto>>() ?? [];
        }

        return [];
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid productId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.DeleteAsync(new Uri($"/api/v1/productCategories/{productId}", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        return Result.Deleted;
    }
}
