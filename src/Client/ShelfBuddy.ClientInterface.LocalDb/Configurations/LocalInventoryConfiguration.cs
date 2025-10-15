using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelfBuddy.ClientInterface.LocalDb.Entities;

namespace ShelfBuddy.ClientInterface.LocalDb.Configurations;

public class LocalInventoryConfiguration : IEntityTypeConfiguration<LocalInventory>
{
    public void Configure(EntityTypeBuilder<LocalInventory> builder)
    {
        builder.HasKey(inventory => inventory.Id);
        builder.Property(inventory => inventory.Id).ValueGeneratedNever();

        builder.Property(inventory => inventory.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(inventory => inventory.UserId)
            .IsRequired();

        builder.HasMany(inventory => inventory.Products)
            .WithMany();

        builder.Property(inventory => inventory.IsSynced)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(inventory => inventory.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(inventory => inventory.LastUpdated)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(inventory => inventory.UserId);
        builder.HasIndex(inventory => inventory.IsSynced);
    }
}