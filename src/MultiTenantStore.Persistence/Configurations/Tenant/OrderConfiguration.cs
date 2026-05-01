using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Persistence.Configurations.Tenant;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("ORDER");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.Property(x => x.CustomerId)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PaymentStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ShippingStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.ShippingAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.ShippingFullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ShippingPhone)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ShippingCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingDistrict)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.ShippingAddressLine1)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ShippingAddressLine2)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.ShippingPostalCode)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.BillingFullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.BillingPhone)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.BillingCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BillingCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BillingDistrict)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.BillingAddressLine1)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.BillingAddressLine2)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.BillingPostalCode)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Payments)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Invoice)
            .WithOne(x => x.Order)
            .HasForeignKey<Invoice>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}