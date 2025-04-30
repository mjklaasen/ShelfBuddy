using MassTransit;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public class CreateInventoryConsumer(IInventoryRepository inventoryRepository) : IConsumer<CreateInventory>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

    public async Task Consume(ConsumeContext<CreateInventory> context)
    {
        var inventory  = new Inventory();
        var result = await _inventoryRepository.CreateAsync(inventory);
        if (result.IsError)
        {
            await context.RespondAsync(new ErrorResponse(result.Errors));
            return;
        }

        await context.RespondAsync(new InventoryCreated(inventory.Id));
    }
}