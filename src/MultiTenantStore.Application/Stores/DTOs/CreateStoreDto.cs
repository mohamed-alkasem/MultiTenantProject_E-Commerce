namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class CreateStoreDto
{
    public Guid OwnerUserId { get; set; }

    public string StoreName { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string PlanCode { get; set; } = default!;

    public string BillingCycle { get; set; } = default!;
}