using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Configurations.Platform;

public class StoreSubscriptionConfiguration : IEntityTypeConfiguration<StoreSubscription>
{
    public void Configure(EntityTypeBuilder<StoreSubscription> builder)
    {
        builder.ToTable("STORE_SUBSCRIPTION");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreId)
            .IsRequired();

        builder.Property(x => x.PlanId)
            .IsRequired();

        builder.Property(x => x.BillingCycle)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.AutoRenew)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired(false);

        builder.Property(x => x.TrialEndsAt)
            .IsRequired(false);

        builder.Property(x => x.CancelledAt)
            .IsRequired(false);

        builder.Property(x => x.RenewedAt)
            .IsRequired(false);

        builder.Property(x => x.NextBillingAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Store)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Plan)
            .WithMany(x => x.StoreSubscriptions)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}