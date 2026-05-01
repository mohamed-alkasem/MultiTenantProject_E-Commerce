using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Platform;

public class SubscriptionPlan : AuditableEntity, ISoftDelete
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;

    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }

    public int MaxProducts { get; set; }
    public int MaxStaffUsers { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? DeletedAt { get; set; }

    public ICollection<StoreSubscription> StoreSubscriptions { get; set; } = new List<StoreSubscription>();
}