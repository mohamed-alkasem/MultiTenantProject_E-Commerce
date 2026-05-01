using MultiTenantStore.Application.Common.MultiTenancy;

namespace MultiTenantStore.Infrastructure.MultiTenancy;

public sealed class CurrentTenant : ICurrentTenant
{
    private TenantInfo? _tenant;
    private string? _storeRole;

    public bool IsResolved => _tenant is not null;

    public Guid? StoreId => _tenant?.StoreId;

    public string? StoreName => _tenant?.StoreName;

    public string? Slug => _tenant?.Slug;

    public string? Subdomain => _tenant?.Subdomain;

    public string? ConnectionString => _tenant?.ConnectionString;

    public string? StoreRole => _storeRole;

    public void SetTenant(TenantInfo tenant)
    {
        _tenant = tenant;
    }

    public void SetStoreRole(string? role)
    {
        _storeRole = role;
    }

    public void Clear()
    {
        _tenant = null;
        _storeRole = null;
    }
}