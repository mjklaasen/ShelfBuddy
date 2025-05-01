using ErrorOr;
using MassTransit;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public class CreateInventoryConsumer(IInventoryRepository inventoryRepository) : IConsumer<CreateInventory>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

    public async Task Consume(ConsumeContext<CreateInventory> context)
    {
        var inventory  = new Inventory(context.Message.Name, context.Message.UserId);
        var created = await _inventoryRepository.CreateAsync(inventory);
        if (created == 0)
        {
            await context.RespondAsync(new ErrorResponse([
                Error.Failure(code: "Inventory.Application.NotCreated", description: "Nothing was created.")
            ]));
            return;
        }

        await context.RespondAsync(new InventoryCreated(inventory.Id));
    }
}