namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class UpdateStoreDto
{
    public string StoreName { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string Status { get; set; } = default!;
}