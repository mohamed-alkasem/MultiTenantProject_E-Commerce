using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Platform;

public class StoreBranding : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }

    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Store Store { get; set; } = default!;
}