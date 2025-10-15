using ErrorOr;
using MassTransit;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public class CreateProductConsumer(
    IProductRepository productRepository,
    IProductCategoryRepository productCategoryRepository) : IConsumer<CreateProduct>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository = productCategoryRepository;

    public async Task Consume(ConsumeContext<CreateProduct> context)
    {
        var product = new Product(context.Message.Name, context.Message.ProductCategory);
        var created = await _productRepository.CreateAsync(product);
        if (created == 0)
        {
            await context.RespondAsync(new ErrorResponse(
                [Error.Failure(code: "Product.Application.NotCreated", description: "Nothing was created.")]));
            return;
        }

        await context.RespondAsync(new ProductCreated(new ProductDto(product.Id, product.Name,
            new ProductCategoryDto(product.ProductCategory.Id, product.ProductCategory.Name,
                _productCategoryRepository.GetLastUpdated(product.ProductCategory)),
            _productRepository.GetLastUpdated(product))));
    }
}