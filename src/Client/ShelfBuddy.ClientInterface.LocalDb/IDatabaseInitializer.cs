namespace ShelfBuddy.ClientInterface.LocalDb;

public interface IDatabaseInitializer
{
    Task InitializeDatabaseAsync();
}