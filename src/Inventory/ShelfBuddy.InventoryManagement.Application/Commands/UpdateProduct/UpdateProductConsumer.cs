using ErrorOr;
using MassTransit;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public class UpdateProductConsumer(IProductRepository productRepository, IProductCategoryRepository productCategoryRepository) : IConsumer<UpdateProduct>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository = productCategoryRepository;

    public async Task Consume(ConsumeContext<UpdateProduct> context)
    {
        var product = await _productRepository.GetByIdAsync(context.Message.Id);
        if (product is null)
        {
            await context.RespondAsync(new ErrorResponse([
                Error.NotFound(code: "Product.Application.NotFound", description: "Product not found.")
            ]));
            return;
        }

        product.Name = context.Message.Name;

        if (!product.ProductCategory.Name.Equals(context.Message.ProductCategory, StringComparison.OrdinalIgnoreCase))
        {
            await UpdateProductCategoryAsync(context.Message, product);
        }

        await _productRepository.UpdateAsync(product);

        await context.RespondAsync(new ProductUpdated(new ProductDto(product.Id, product.Name,
            new ProductCategoryDto(product.ProductCategory.Id, product.ProductCategory.Name))));
    }

    private async Task UpdateProductCategoryAsync(UpdateProduct message, Product product)
    {
        var newProductCategory = await _productCategoryRepository.GetByNameAsync(message.ProductCategory);
        if (newProductCategory is not null)
        {
            product.UpdateProductCategory(newProductCategory);
            return;
        }

        product.UpdateProductCategory(message.ProductCategory);
        await _productCategoryRepository.CreateAsync(product.ProductCategory);
    }
}