using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Configurations;

public class LocalProductConfiguration : IEntityTypeConfiguration<LocalProduct>
{
    public void Configure(EntityTypeBuilder<LocalProduct> builder)
    {
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).ValueGeneratedNever();

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(product => product.IsSynced)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(product => product.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(product => product.LastUpdated)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(product => product.ProductCategory)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(product => product.IsSynced);
    }
}