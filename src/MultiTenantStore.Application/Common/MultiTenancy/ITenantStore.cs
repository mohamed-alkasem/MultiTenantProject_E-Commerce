namespace MultiTenantStore.Application.Common.MultiTenancy;

public interface ITenantStore
{
    Task<TenantInfo?> FindByStoreIdAsync(
        Guid storeId,
        CancellationToken cancellationToken = default);

    Task<TenantInfo?> FindBySubdomainAsync(
        string subdomain,
        CancellationToken cancellationToken = default);
}