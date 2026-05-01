using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Configurations.Platform;

public class StoreDatabaseConfiguration : IEntityTypeConfiguration<StoreDatabase>
{
    public void Configure(EntityTypeBuilder<StoreDatabase> builder)
    {
        builder.ToTable("STORE_DATABASE");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreId)
            .IsRequired();

        builder.Property(x => x.DatabaseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DbServer)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Provider)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ConnectionStringEncrypted)
            .IsRequired();

        builder.Property(x => x.ProvisioningStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsProvisioned)
            .IsRequired();

        builder.Property(x => x.ProvisionedAt)
            .IsRequired(false);

        builder.Property(x => x.MigrationVersion)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.LastMigrationAt)
            .IsRequired(false);

        builder.Property(x => x.LastError)
            .HasMaxLength(2000)
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
            .WithOne(x => x.Database)
            .HasForeignKey<StoreDatabase>(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}