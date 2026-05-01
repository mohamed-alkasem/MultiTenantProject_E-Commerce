using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Configurations.Platform;

public class StoreBrandingConfiguration : IEntityTypeConfiguration<StoreBranding>
{
    public void Configure(EntityTypeBuilder<StoreBranding> builder)
    {
        builder.ToTable("STORE_BRANDING");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreId)
            .IsRequired();

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.PrimaryColor)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.SecondaryColor)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.ContactEmail)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.ContactPhone)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.Address)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(x => x.StoreId)
            .IsUnique();

        builder.HasOne(x => x.Store)
            .WithOne(x => x.Branding)
            .HasForeignKey<StoreBranding>(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}