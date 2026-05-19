using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Persistence.Configurations.Tenant;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("PRODUCT");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.NameAr)
            .HasMaxLength(300)
            .IsRequired(false);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.ShortDescription)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.ShortDescriptionAr)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.Description)
            .IsRequired(false);

        builder.Property(x => x.DescriptionAr)
            .IsRequired(false);

        builder.Property(x => x.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.SKU)
            .IsUnique();

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

        builder.Property(x => x.LowStockThreshold)
            .IsRequired(false);

        builder.Property(x => x.IsFeatured)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.Property(x => x.PublishedAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Variants)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}