namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class StoreDomainDto
{
    public Guid Id { get; set; }

    public Guid StoreId { get; set; }

    public string? Subdomain { get; set; }

    public string FullDomain { get; set; } = default!;

    public bool IsPrimary { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? SslStatus { get; set; }
}