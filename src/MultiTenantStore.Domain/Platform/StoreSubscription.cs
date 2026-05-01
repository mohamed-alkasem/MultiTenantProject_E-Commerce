using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Platform;

public class StoreSubscription : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }
    public Guid PlanId { get; set; }

    public BillingCycle BillingCycle { get; set; }
    public SubscriptionStatus Status { get; set; }

    public bool AutoRenew { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public DateTime? TrialEndsAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? RenewedAt { get; set; }
    public DateTime? NextBillingAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Store Store { get; set; } = default!;
    public SubscriptionPlan Plan { get; set; } = default!;

    public ICollection<SubscriptionPayment> Payments { get; set; } = new List<SubscriptionPayment>();
}