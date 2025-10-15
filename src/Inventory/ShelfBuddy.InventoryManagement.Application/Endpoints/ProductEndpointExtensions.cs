using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ShelfBuddy.API.Common;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

internal static class ProductEndpointExtensions
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/products")
            .WithGroupName("Products")
            .WithTags("Products");

        group.MapPost("/",
                async ([FromBody] CreateProduct message, [FromServices] IRequestClient<CreateProduct> client) =>
                {
                    var response = await client.GetResponse<ProductCreated, ErrorResponse>(message);
                    return response switch
                    {
                        { Message: ProductCreated created } => Results.CreatedAtRoute("GetProduct",
                            new { id = created.Product.Id }, created.Product),
                        { Message: ErrorResponse errorResponse } => CustomResults.Problem(errorResponse.Errors),
                        _ => Results.Problem("An unknown error occurred.")
                    };
                })
            .WithName("CreateProduct");

        group.MapGet("/{id:guid}", async ([FromServices] IProductRepository productRepository, Guid id) =>
            {
                var product = await productRepository.GetByIdAsync(id);
                return product is null
                    ? Results.NotFound()
                    : Results.Ok(new ProductDto(product.Id, product.Name, new ProductCategoryDto(product.ProductCategory.Id, product.ProductCategory.Name)));
            })
            .WithName("GetProduct");

        group.MapGet("/", async (HttpContext context, [FromServices] IProductRepository productRepository,
                [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? productCategory = null) =>
            {
                var listProductsResult = await productRepository.ListAsync(page, pageSize, productCategory);
                var products = listProductsResult as List<Product> ?? listProductsResult.ToList();
                context.Response.Headers.Append("X-Total-Count", (await productRepository.CountAsync()).ToString());
                return Results.Ok(products.Select(x =>
                    new ProductDto(x.Id, x.Name,
                        new ProductCategoryDto(x.ProductCategory.Id, x.ProductCategory.Name))));
            })
            .WithName("ListProducts");

        group.MapPut("/{id:guid}",
            async ([FromBody] ProductDto message, [FromServices] IRequestClient<UpdateProduct> client, Guid id) =>
            {
                var response =
                    await client.GetResponse<ProductUpdated, ErrorResponse>(new UpdateProduct(id, message.Name,
                        message.ProductCategory.Name));
                return response switch
                {
                    { Message: ProductUpdated updated } => Results.Ok(updated.Product),
                    { Message: ErrorResponse errorResponse } => CustomResults.Problem(errorResponse.Errors),
                    _ => Results.Problem("An unknown error occurred.")
                };
            }).WithName("UpdateProduct");

        group.MapDelete("/{id:guid}", async ([FromServices] IProductRepository productRepository, Guid id) =>
        {
            await productRepository.DeleteAsync(id);
            return Results.Ok();
        });

        return app;
    }
}