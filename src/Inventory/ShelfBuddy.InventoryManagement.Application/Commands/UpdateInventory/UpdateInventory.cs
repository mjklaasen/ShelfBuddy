namespace ShelfBuddy.InventoryManagement.Application;

public record UpdateInventory(Guid Id, string Name, Guid UserId, Dictionary<Guid, int> Products);