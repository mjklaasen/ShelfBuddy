using ErrorOr;
using MassTransit;
using ShelfBuddy.Contracts;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Application;

public class UpdateInventoryConsumer(IInventoryRepository inventoryRepository, IProductRepository productRepository) : IConsumer<UpdateInventory>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Consume(ConsumeContext<UpdateInventory> context)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(context.Message.Id);
        if (inventory is null)
        {
            await context.RespondAsync(new ErrorResponse(
                [Error.NotFound(code: "Inventory.Application.NotFound", description: "Inventory not found.")]));
            return;
        }

        var updateResult = await UpdateInventoryAsync(context.Message, inventory);
        if (updateResult.IsError)
        {
            await context.RespondAsync(new ErrorResponse(updateResult.Errors));
            return;
        }

        var updated = await _inventoryRepository.UpdateAsync(inventory);
        if (updated == 0)
        {
            await context.RespondAsync(new ErrorResponse(
                [Error.Failure(code: "Inventory.Application.NotUpdated", description: "Nothing was updated.")]));
            return;
        }

        await context.RespondAsync(new InventoryUpdated());
    }

    private async Task<ErrorOr<Updated>> UpdateInventoryAsync(UpdateInventory message, Inventory inventory)
    {
        inventory.Name = message.Name;
        inventory.UserId = message.UserId;
        List<Error> errors = [];

        foreach (var (productKey, quantity) in message.Products)
        {
            if (!inventory.Products.ContainsKey(productKey))
            {
                if (await _productRepository.GetByIdAsync(productKey) is null)
                {
                    errors.Add(Error.NotFound(code: "Inventory.Application.ProductNotFound",
                        description: "Product not found."));
                    continue;
                }
            }

            var currentQuantity = inventory.GetProductQuantity(productKey);
            if (currentQuantity < quantity)
            {
                inventory.AddProduct(productKey, quantity - currentQuantity);
                continue;
            }

            if (currentQuantity > quantity)
            {
                inventory.RemoveProduct(productKey, currentQuantity - quantity);
            }
        }

        return errors.Count > 0
            ? errors
            : Result.Updated;
    }
}