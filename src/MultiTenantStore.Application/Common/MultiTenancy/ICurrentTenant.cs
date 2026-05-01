namespace MultiTenantStore.Application.Common.MultiTenancy;

public interface ICurrentTenant
{
    bool IsResolved { get; }

    Guid? StoreId { get; }

    string? StoreName { get; }

    string? Slug { get; }

    string? Subdomain { get; }

    string? ConnectionString { get; }

    string? StoreRole { get; }

    void SetTenant(TenantInfo tenant);

    void SetStoreRole(string? role);

    void Clear();
}