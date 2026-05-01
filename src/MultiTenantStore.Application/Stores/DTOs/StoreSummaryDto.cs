namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class StoreSummaryDto
{
    public Guid Id { get; set; }

    public string StoreName { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string Status { get; set; } = default!;

    public string SubscriptionStatus { get; set; } = default!;

    public string? PrimaryDomain { get; set; }
}