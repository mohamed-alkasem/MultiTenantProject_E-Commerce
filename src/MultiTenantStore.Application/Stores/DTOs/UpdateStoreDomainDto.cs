namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class UpdateStoreDomainDto
{
    public Guid DomainId { get; set; }

    public bool IsPrimary { get; set; }
}