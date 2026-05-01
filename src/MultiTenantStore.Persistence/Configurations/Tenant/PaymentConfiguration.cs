using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Persistence.Configurations.Tenant;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("PAYMENT");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.RefundedAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PaymentProvider)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProviderReference)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.TransactionId)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.ProviderResponseCode)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.PaidAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(x => x.ProviderReference);

        builder.HasIndex(x => x.TransactionId);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}