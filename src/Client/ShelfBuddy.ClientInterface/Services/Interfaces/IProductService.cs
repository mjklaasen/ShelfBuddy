using ErrorOr;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.ClientInterface.Services;

public interface IProductService
{
    Task<ErrorOr<ProductDto>> GetAsync(Guid id);
    Task<List<ProductDto>> ListAsync(int page = 1, int pageSize = 10);
    Task<ErrorOr<ProductDto>> CreateAsync(string name, string category);
    Task<ErrorOr<Updated>> UpdateAsync(ProductDto product);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid productId);
}