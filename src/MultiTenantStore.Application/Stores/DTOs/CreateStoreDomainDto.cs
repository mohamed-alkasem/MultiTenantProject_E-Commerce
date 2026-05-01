namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class CreateStoreDomainDto
{
    public Guid StoreId { get; set; }

    public string? Subdomain { get; set; }

    public string? FullDomain { get; set; }

    public bool IsPrimary { get; set; }
}