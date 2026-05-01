using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Platform;

public class StoreDomain : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }

    public string? Subdomain { get; set; }
    public string FullDomain { get; set; } = default!;

    public bool IsPrimary { get; set; }
    public bool IsVerified { get; set; }

    public string? VerificationToken { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? SslStatus { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Store Store { get; set; } = default!;
}