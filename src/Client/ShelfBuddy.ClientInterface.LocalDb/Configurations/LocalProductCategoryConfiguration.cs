using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Configurations;

public class LocalProductCategoryConfiguration : IEntityTypeConfiguration<LocalProductCategory>
{
    public void Configure(EntityTypeBuilder<LocalProductCategory> builder)
    {
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).ValueGeneratedNever();

        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(category => category.IsSynced)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(category => category.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(category => category.LastUpdated)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(category => category.Name).IsUnique();
        builder.HasIndex(category => category.IsSynced);
    }
}