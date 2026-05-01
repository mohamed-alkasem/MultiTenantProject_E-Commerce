namespace MultiTenantStore.Application.Common.MultiTenancy;

public sealed class TenantInfo
{
    public Guid StoreId { get; set; }

    public string StoreName { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string? Subdomain { get; set; }

    public string ConnectionString { get; set; } = default!;

    public string StoreStatus { get; set; } = default!;

    public string SubscriptionStatus { get; set; } = default!;

    public bool IsActive => StoreStatus.Equals("Active", StringComparison.OrdinalIgnoreCase);
}