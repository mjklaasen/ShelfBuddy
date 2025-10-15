using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ShelfBuddy.ClientInterface.LocalDb;

public class DatabaseInitializer(LocalDbContext dbContext) : IDatabaseInitializer
{
    private readonly LocalDbContext _dbContext = dbContext;

    public async Task InitializeDatabaseAsync()
    {
        using var activitySource = new ActivitySource("Migrations");
        using var activity = activitySource.StartActivity(nameof(DatabaseInitializer), ActivityKind.Client);
        try
        {
            await EnsureDatabaseAsync(_dbContext);
            await RunMigrationAsync(_dbContext);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }
    }

    internal static async Task EnsureDatabaseAsync(DbContext dbContext)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            try
            {
                if (!await dbCreator.ExistsAsync())
                {
                    await dbCreator.CreateAsync();
                }
            }
            catch (Exception ex)
            {
                // Ignore if the database already exists.
                if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                throw;
            }

        });
    }

    internal static async Task RunMigrationAsync(DbContext dbContext)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToArray();
            if (pendingMigrations.Length == 0)
            {
                return;
            }
            await dbContext.Database.MigrateAsync();
            await dbContext.SaveChangesAsync();
        });
    }
}
