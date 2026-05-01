using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Persistence.Configurations.Tenant;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CUSTOMER_ADDRESS");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.District)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.PostalCode)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.IsDefaultShipping)
            .IsRequired();

        builder.Property(x => x.IsDefaultBilling)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}