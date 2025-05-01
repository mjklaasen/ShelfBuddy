using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelfBuddy.InventoryManagement.Domain;

namespace ShelfBuddy.InventoryManagement.Infrastructure.Persistence.Configurations;

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