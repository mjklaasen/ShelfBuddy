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
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
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
    }

    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
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

        return app;
    }
}