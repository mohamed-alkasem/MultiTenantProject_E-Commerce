using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Persistence.Configurations.Tenant;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("INVOICE");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.InvoiceNumber)
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IssueDate)
            .IsRequired();

        builder.Property(x => x.DueDate)
            .IsRequired(false);

        builder.Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.ShippingAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.CustomerNameSnapshot)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CustomerEmailSnapshot)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.BillingAddressSnapshot)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.StoreNameSnapshot)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.StoreTaxNumberSnapshot)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.StoreAddressSnapshot)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.PdfUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Order)
            .WithOne(x => x.Invoice)
            .HasForeignKey<Invoice>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}