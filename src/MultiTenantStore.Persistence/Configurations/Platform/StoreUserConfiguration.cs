using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Configurations.Platform;

public class StoreUserConfiguration : IEntityTypeConfiguration<StoreUser>
{
    public void Configure(EntityTypeBuilder<StoreUser> builder)
    {
        builder.ToTable("STORE_USER");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StoreId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.InvitedAt)
            .IsRequired(false);

        builder.Property(x => x.JoinedAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(x => new { x.StoreId, x.UserId })
            .IsUnique();

        builder.HasOne(x => x.Store)
            .WithMany(x => x.StoreUsers)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.StoreUsers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}