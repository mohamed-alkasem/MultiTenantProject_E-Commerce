using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Configurations.Platform;

public class SubscriptionPaymentConfiguration : IEntityTypeConfiguration<SubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
    {
        builder.ToTable("SUBSCRIPTION_PAYMENT");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreSubscriptionId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.PaymentProvider)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProviderReference)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.TransactionId)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.InvoiceNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.PaymentStatus)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000)
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

        builder.HasIndex(x => x.InvoiceNumber);

        builder.HasOne(x => x.StoreSubscription)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.StoreSubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}