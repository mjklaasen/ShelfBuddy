using System.Globalization;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ShelfBuddy.API.Common;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public static class InventoryEndpointExtensions
{
    internal static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/inventories")
            .WithGroupName("Inventories")
            .WithTags("Inventories");

        group.MapPost("/",
                async ([FromBody] CreateInventory message, [FromServices] IRequestClient<CreateInventory> client) =>
                {
                    var response = await client.GetResponse<InventoryCreated, ErrorResponse>(message);
                    return response switch
                    {
                        { Message: InventoryCreated created } => Results.CreatedAtRoute("GetInventory",
                            new { id = created.Id }, new InventoryDto(created.Id, message.Name, message.UserId, [], created.CreatedAt)),
                        { Message: ErrorResponse errorResponse } => CustomResults.Problem(errorResponse.Errors),
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
                        { Message: InventoryUpdated } =>
                            Results.Ok(message.Id is null ? message with { Id = id } : message),
                        { Message: ErrorResponse errorResponse } => CustomResults.Problem(errorResponse.Errors),
                        _ => Results.Problem("An unknown error occurred.")
                    };
                })
            .WithName("UpdateInventory");

        group.MapGet("/",
                async (HttpContext context, [FromServices] IInventoryRepository inventoryRepository, [FromQuery] int page = 1,
                    [FromQuery] int pageSize = 10, [FromQuery] Guid? userId = null) =>
                {
                    var listInventoriesResult = await inventoryRepository.ListAsync(page, pageSize, userId);
                    var inventories = listInventoriesResult as List<Inventory> ?? listInventoriesResult.ToList();
                    context.Response.Headers.Append("X-Total-Count",
                        (await inventoryRepository.CountAsync(userId)).ToString(CultureInfo.InvariantCulture));
                    return Results.Ok(inventories.Select(x => new InventoryDto(x.Id, x.Name, x.UserId,
                        x.Products.ToDictionary(), inventoryRepository.GetLastUpdated(x))));
                })
            .WithName("ListInventories");

        group.MapGet("/{id:guid}", async ([FromServices] IInventoryRepository inventoryRepository, Guid id) =>
        {
            var inventory = await inventoryRepository.GetByIdAsync(id);
            return inventory is null
                ? Results.NotFound()
                : Results.Ok(new InventoryDto(inventory.Id, inventory.Name, inventory.UserId,
                    inventory.Products.ToDictionary(), inventoryRepository.GetLastUpdated(inventory)));
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
}
