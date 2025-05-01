using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ShelfBuddy.API.Common;
using ShelfBuddy.Contracts;

namespace ShelfBuddy.InventoryManagement.Application;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapInventoryManagementEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapInventoryEndpoints()
            .MapProductEndpoints();
    }

    private static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/inventory")
            .WithGroupName("Inventory")
            .WithTags("Inventory");

        group.MapPost("/",
                async ([FromBody] CreateInventory message, [FromServices] IRequestClient<CreateInventory> client) =>
                {
                    var response = await client.GetResponse<InventoryCreated, ErrorResponse>(message);
                    return response switch
                    {
                        {Message: InventoryCreated created} => Results.CreatedAtRoute("GetInventory",
                            new {id = created.Id}, new InventoryDto(created.Id, message.Name, message.UserId, [])),
                        {Message: ErrorResponse errorResponse} => CustomResults.Problem(errorResponse.Errors),
                        _ => Results.Problem("An unknown error occurred.")
                    };
                })
            .WithName("CreateInventory");

        group.MapPut("/{id:guid}",
                async ([FromBody] InventoryDto message, [FromServices] IRequestClient<UpdateInventory> client, Guid id) =>
                {
                    var response =
                        await client.GetResponse<InventoryUpdated, ErrorResponse>(
                            new UpdateInventory(id, message.Name, message.UserId, message.Products));
                    return response switch
                    {
                        {Message: InventoryUpdated} =>
                            Results.Ok(message.Id is null ? message with {Id = id} : message),
                        {Message: ErrorResponse errorResponse} => CustomResults.Problem(errorResponse.Errors),
                        _ => Results.Problem("An unknown error occurred.")
                    };
                })
            .WithName("UpdateInventory");

        group.MapGet("/", async ([FromServices] IInventoryRepository inventoryRepository,
                [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
            {
                var inventories = await inventoryRepository.ListAsync();
                return Results.Ok(inventories.Select(x =>
                    new InventoryDto(x.Id, x.Name, x.UserId, x.Products.ToDictionary())));
            })
            .WithName("ListInventories");

        group.MapGet("/{id:guid}", async ([FromServices] IInventoryRepository inventoryRepository, Guid id) =>
            {
                var inventory = await inventoryRepository.GetByIdAsync(id);
                return inventory is null
                    ? Results.NotFound()
                    : Results.Ok(new InventoryDto(inventory.Id, inventory.Name, inventory.UserId,
                        inventory.Products.ToDictionary()));
            })
            .WithName("GetInventory");

        group.MapDelete("/{id:guid}",
                async ([FromServices] IInventoryRepository inventoryRepository, Guid id) =>
                {
                    await inventoryRepository.DeleteAsync(id);
                    return Results.Ok();
                })
            .WithName("DeleteInventory");

        return app;
    }

    private static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/product")
            .WithGroupName("Product")
            .WithTags("Product");

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

        return app;
    }
}