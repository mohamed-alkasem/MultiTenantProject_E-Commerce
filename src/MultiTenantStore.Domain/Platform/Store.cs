using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Platform;

public class Store : AuditableEntity, ISoftDelete
{
    public Guid OwnerUserId { get; set; }

    public string StoreName { get; set; } = default!;
    public string Slug { get; set; } = default!;

    public StoreStatus Status { get; set; } = StoreStatus.PendingProvisioning;
    public SubscriptionStatus SubscriptionStatus { get; set; } = SubscriptionStatus.Trial;

    public DateTime? ActivatedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ApplicationUser OwnerUser { get; set; } = default!;

    public ICollection<StoreUser> StoreUsers { get; set; } = new List<StoreUser>();
    public ICollection<StoreDomain> Domains { get; set; } = new List<StoreDomain>();
    public ICollection<StoreSubscription> Subscriptions { get; set; } = new List<StoreSubscription>();

    public StoreDatabase? Database { get; set; }
    public StoreBranding? Branding { get; set; }
}