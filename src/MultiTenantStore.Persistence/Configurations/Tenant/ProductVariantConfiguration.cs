using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Persistence.Configurations.Tenant;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("PRODUCT_VARIANT");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.SKU)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.NameAr)
            .HasMaxLength(300)
            .IsRequired(false);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.CompareAtPrice)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(x => x.CostPrice)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(x => x.StockQuantity)
            .IsRequired();

        builder.Property(x => x.TrackInventory)
            .IsRequired();

        builder.Property(x => x.AttributesJson)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}