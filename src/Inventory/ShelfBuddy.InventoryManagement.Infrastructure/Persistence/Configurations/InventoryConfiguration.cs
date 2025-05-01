using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = JsonSerializerOptions.Web;

    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(inventory => inventory.Id);
        builder.Property(inventory => inventory.Id).ValueGeneratedNever();

        builder.Property(inventory => inventory.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(inventory => inventory.UserId);
        builder.Property(typeof(Dictionary<Guid, int>), "_products").HasConversion(
                new ValueConverter<Dictionary<Guid, int>, string>(
                    x => JsonSerializer.Serialize(x, JsonSerializerOptions),
                    x => JsonSerializer.Deserialize<Dictionary<Guid, int>>(x, JsonSerializerOptions) ??
                         new Dictionary<Guid, int>()), ValueComparer.CreateDefault<Dictionary<Guid, int>>(true))
            .HasColumnName("Products")
            .HasDefaultValue(new Dictionary<Guid, int>());
        builder.Ignore(inventory => inventory.Products);
    }
}