using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedNever();

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(product => product.ProductCategory)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }
}