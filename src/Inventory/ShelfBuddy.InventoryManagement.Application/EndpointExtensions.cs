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
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/inventory",
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
}