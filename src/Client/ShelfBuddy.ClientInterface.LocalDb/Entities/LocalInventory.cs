namespace ShelfBuddy.ClientInterface.LocalDb.Entities;

public class LocalInventory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public IList<LocalProduct> Products { get; init; } = [];
    public bool IsSynced { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
}
