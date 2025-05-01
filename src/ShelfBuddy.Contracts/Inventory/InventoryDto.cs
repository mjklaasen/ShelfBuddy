namespace ShelfBuddy.Contracts;

public record InventoryDto(Guid? Id, string Name, Guid UserId, Dictionary<Guid, int> Products);