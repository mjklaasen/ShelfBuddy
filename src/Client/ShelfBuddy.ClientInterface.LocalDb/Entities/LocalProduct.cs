namespace ShelfBuddy.ClientInterface.LocalDb.Entities;

public class LocalProduct
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public required LocalProductCategory ProductCategory { get; set; }
    public bool IsSynced { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
}
