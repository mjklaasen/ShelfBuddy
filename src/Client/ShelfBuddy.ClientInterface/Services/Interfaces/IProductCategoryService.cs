using ErrorOr;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.ClientInterface.Services;

public interface IProductCategoryService
{
    Task<ErrorOr<ProductCategoryDto>> GetByIdAsync(Guid id);
    Task<ErrorOr<ProductCategoryDto>> GetByNameAsync(string name);
    Task<List<ProductCategoryDto>> ListAsync(int page = 1, int pageSize = 10);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid productId);
}