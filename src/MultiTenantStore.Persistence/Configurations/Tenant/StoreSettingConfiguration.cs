using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Persistence.Configurations.Tenant;

public class StoreSettingConfiguration : IEntityTypeConfiguration<StoreSetting>
{
    public void Configure(EntityTypeBuilder<StoreSetting> builder)
    {
        builder.ToTable("STORE_SETTING");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Timezone)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DefaultLanguage)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.IsCheckoutEnabled)
            .IsRequired();

        builder.Property(x => x.TaxEnabled)
            .IsRequired();

        builder.Property(x => x.OrderPrefix)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);
    }
}