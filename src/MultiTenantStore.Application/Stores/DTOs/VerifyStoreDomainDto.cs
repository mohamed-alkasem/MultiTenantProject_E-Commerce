namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class VerifyStoreDomainDto
{
    public Guid DomainId { get; set; }

    public string VerificationToken { get; set; } = default!;
}