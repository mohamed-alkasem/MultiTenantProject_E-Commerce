namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class CancelStoreSubscriptionDto
{
    public Guid StoreSubscriptionId { get; set; }

    public string Reason { get; set; } = default!;
}