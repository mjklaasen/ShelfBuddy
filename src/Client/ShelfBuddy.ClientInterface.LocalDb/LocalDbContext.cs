using Microsoft.EntityFrameworkCore;
using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb;

public class LocalDbContext(DbContextOptions<LocalDbContext> options) : DbContext(options)
{
    public required DbSet<LocalInventory> Inventories { get; set; }
    public required DbSet<LocalProduct> Products { get; set; }
    public required DbSet<LocalProductCategory> ProductCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocalDbContext).Assembly);
    }
}