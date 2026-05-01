namespace MultiTenantStore.Infrastructure.MultiTenancy;

public sealed class TenantResolutionOptions
{
    public string BaseDomain { get; set; } = "localhost";

    public string DashboardHost { get; set; } = "localhost";

    public bool EnableSubdomainResolution { get; set; } = true;

    public bool EnableClaimsResolution { get; set; } = true;

    public List<string> ExcludedPaths { get; set; } = new();

    public List<string> DashboardPaths { get; set; } = new();

    public List<string> PublicTenantPaths { get; set; } = new();
}