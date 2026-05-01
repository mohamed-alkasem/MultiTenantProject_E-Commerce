namespace MultiTenantStore.Application.Common.MultiTenancy;

public sealed class TenantResolutionResult
{
    public bool Success { get; private set; }

    public Guid? StoreId { get; private set; }

    public Guid? UserId { get; private set; }

    public string? TenantKey { get; private set; }

    public string? StoreRole { get; private set; }

    public string Source { get; private set; } = default!;

    public string? Error { get; private set; }

    public static TenantResolutionResult FromClaims(
        Guid storeId,
        Guid userId,
        string? storeRole)
    {
        return new TenantResolutionResult
        {
            Success = true,
            StoreId = storeId,
            UserId = userId,
            StoreRole = storeRole,
            Source = "Claims"
        };
    }

    public static TenantResolutionResult FromSubdomain(string subdomain)
    {
        return new TenantResolutionResult
        {
            Success = true,
            TenantKey = subdomain,
            Source = "Subdomain"
        };
    }

    public static TenantResolutionResult Failed(string source, string error)
    {
        return new TenantResolutionResult
        {
            Success = false,
            Source = source,
            Error = error
        };
    }
}