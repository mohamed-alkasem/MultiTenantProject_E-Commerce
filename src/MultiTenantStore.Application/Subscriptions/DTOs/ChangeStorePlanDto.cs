namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class ChangeStorePlanDto
{
    public Guid StoreId { get; set; }

    public Guid NewPlanId { get; set; }

    public string BillingCycle { get; set; } = default!;
}