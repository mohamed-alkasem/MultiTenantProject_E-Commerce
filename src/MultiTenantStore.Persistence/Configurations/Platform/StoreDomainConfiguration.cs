using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Configurations.Platform;

public class StoreDomainConfiguration : IEntityTypeConfiguration<StoreDomain>
{
    public void Configure(EntityTypeBuilder<StoreDomain> builder)
    {
        builder.ToTable("STORE_DOMAIN");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreId)
            .IsRequired();

        builder.Property(x => x.Subdomain)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.FullDomain)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.IsPrimary)
            .IsRequired();

        builder.Property(x => x.IsVerified)
            .IsRequired();

        builder.Property(x => x.VerificationToken)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.VerifiedAt)
            .IsRequired(false);

        builder.Property(x => x.SslStatus)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(x => x.Subdomain)
            .IsUnique()
            .HasFilter("[Subdomain] IS NOT NULL");

        builder.HasIndex(x => x.FullDomain)
            .IsUnique();

        builder.HasOne(x => x.Store)
            .WithMany(x => x.Domains)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}