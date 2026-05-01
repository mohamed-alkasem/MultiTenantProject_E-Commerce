using MultiTenantStore.Application.Subscriptions.DTOs;

namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class StoreDto
{
    public Guid Id { get; set; }

    public Guid OwnerUserId { get; set; }

    public string StoreName { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string Status { get; set; } = default!;

    public string SubscriptionStatus { get; set; } = default!;

    public DateTime? ActivatedAt { get; set; }

    public DateTime? SuspendedAt { get; set; }

    public StoreBrandingDto? Branding { get; set; }

    public StoreDatabaseDto? Database { get; set; }

    public List<StoreDomainDto> Domains { get; set; } = new();

    public StoreSubscriptionDto? CurrentSubscription { get; set; }
}