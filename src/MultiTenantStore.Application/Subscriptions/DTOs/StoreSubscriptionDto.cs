namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class StoreSubscriptionDto
{
    public Guid Id { get; set; }

    public Guid StoreId { get; set; }

    public Guid PlanId { get; set; }

    public string PlanName { get; set; } = default!;

    public string BillingCycle { get; set; } = default!;

    public string Status { get; set; } = default!;

    public bool AutoRenew { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? TrialEndsAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public DateTime? RenewedAt { get; set; }

    public DateTime? NextBillingAt { get; set; }
}