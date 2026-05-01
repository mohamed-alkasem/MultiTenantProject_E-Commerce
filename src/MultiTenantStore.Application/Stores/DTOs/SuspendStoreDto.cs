namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class SuspendStoreDto
{
    public Guid StoreId { get; set; }

    public string Reason { get; set; } = default!;
}