namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class UpdateStoreDto
{
    public string StoreName { get; set; } = default!;
    public string? StoreNameAr { get; set; }

    public string Slug { get; set; } = default!;

    public string Status { get; set; } = default!;
}