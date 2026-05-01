namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class StoreOnboardingResultDto
{
    public Guid StoreId { get; set; }

    public string StoreName { get; set; } = default!;

    public string StoreSlug { get; set; } = default!;

    public string StoreRole { get; set; } = default!;
}