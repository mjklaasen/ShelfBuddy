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


public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedNever();

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(product => product.Category)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(productCategory => productCategory.Id);
        builder.Property(productCategory => productCategory.Id).ValueGeneratedNever();

        builder.Property(productCategory => productCategory.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}