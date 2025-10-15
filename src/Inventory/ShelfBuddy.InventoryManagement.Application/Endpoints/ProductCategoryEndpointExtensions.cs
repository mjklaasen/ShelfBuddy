using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

internal static class ProductCategoryEndpointExtensions
{
    public static IEndpointRouteBuilder MapProductCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/productCategories")
            .WithGroupName("ProductCategories")
            .WithTags("Product Categories");

        group.MapGet("/{id:guid}", async ([FromServices] IProductCategoryRepository productCategoryRepository, Guid id) =>
        {
            var productCategory = await productCategoryRepository.GetByIdAsync(id);
            return productCategory is null
                ? Results.NotFound()
                : Results.Ok(new ProductCategoryDto(productCategory.Id, productCategory.Name));
        })
            .WithName("GetProductCategoryById");

        group.MapGet("/{name}", async ([FromServices] IProductCategoryRepository productCategoryRepository, string name) =>
        {
            var productCategory = await productCategoryRepository.GetByNameAsync(name);
            return productCategory is null
                ? Results.NotFound()
                : Results.Ok(new ProductCategoryDto(productCategory.Id, productCategory.Name));
        })
            .WithName("GetProductCategoryByName");

        group.MapGet("/", async (HttpContext context, [FromServices] IProductCategoryRepository productCategoryRepository,
                [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? productCategory = null) =>
        {
            var listProductCategoriesResult = await productCategoryRepository.ListAsync(page, pageSize);
            var productCategories = listProductCategoriesResult as List<ProductCategory> ??
                                    listProductCategoriesResult.ToList();
            context.Response.Headers.Append("X-Total-Count", (await productCategoryRepository.CountAsync()).ToString());
            return Results.Ok(productCategories.Select(x => new ProductCategoryDto(x.Id, x.Name)));
        })
            .WithName("ListProductCategories");

        group.MapDelete("/{id:guid}", async ([FromServices] IProductCategoryRepository productCategoryRepository, Guid id) =>
        {
            await productCategoryRepository.DeleteAsync(id);
            return Results.Ok();
        });

        return app;
    }
}