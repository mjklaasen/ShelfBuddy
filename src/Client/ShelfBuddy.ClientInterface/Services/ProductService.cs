using ErrorOr;
using ShelfBuddy.Contracts;
using System.Net.Http.Json;

namespace ShelfBuddy.ClientInterface.Services;

public class ProductService(IHttpClientFactory httpClientFactory) : EntityServiceBase, IProductService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public async Task<ErrorOr<ProductDto>> GetAsync(Guid id)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.GetAsync(new Uri($"/api/v1/products/{id}", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        if (product is null)
        {
            return Error.Failure(description: "Failed to get product: response invalid.");
        }

        return product;
    }

    public async Task<List<ProductDto>> ListAsync(int page = 1, int pageSize = 10)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response =
            await client.GetAsync(new Uri($"/api/v1/products?page={page}&pageSize={pageSize}",
                UriKind.Relative));

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<ProductDto>>() ?? [];
        }

        return [];
    }

    public async Task<ErrorOr<ProductDto>> CreateAsync(string name, string category)
    {
        var client = _httpClientFactory.CreateClient("api");

        var createRequest = new
        {
            name,
            ProductCategory = category
        };

        var response = await client.PostAsJsonAsync(new Uri("/api/v1/products", UriKind.Relative), createRequest);
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        if (product is null)
        {
            return Error.Failure(description: "Failed to create product: response invalid.");
        }

        return product;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(ProductDto product)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.PutAsJsonAsync(new Uri($"/api/v1/products/{product.Id}", UriKind.Relative), product);
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid productId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.DeleteAsync(new Uri($"/api/v1/products/{productId}", UriKind.Relative));
        if (!response.IsSuccessStatusCode)
        {
            return await GetHttpErrorsAsync(response);
        }

        return Result.Deleted;
    }
}
