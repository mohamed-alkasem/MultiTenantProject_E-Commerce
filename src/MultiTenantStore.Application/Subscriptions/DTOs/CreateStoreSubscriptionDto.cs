namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class CreateStoreSubscriptionDto
{
    public Guid StoreId { get; set; }

    public Guid PlanId { get; set; }

    public string BillingCycle { get; set; } = default!;

    public bool AutoRenew { get; set; }
}