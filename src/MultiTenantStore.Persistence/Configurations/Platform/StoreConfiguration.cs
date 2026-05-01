using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Configurations.Platform;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("STORE");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.Property(x => x.StoreName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.SubscriptionStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ActivatedAt)
            .IsRequired(false);

        builder.Property(x => x.SuspendedAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.OwnerUser)
            .WithMany(x => x.OwnedStores)
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.StoreUsers)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Domains)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Subscriptions)
            .WithOne(x => x.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}