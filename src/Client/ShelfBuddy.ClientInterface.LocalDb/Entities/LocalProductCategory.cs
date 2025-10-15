namespace ShelfBuddy.ClientInterface.LocalDb.Entities;

public class LocalProductCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSynced { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
}