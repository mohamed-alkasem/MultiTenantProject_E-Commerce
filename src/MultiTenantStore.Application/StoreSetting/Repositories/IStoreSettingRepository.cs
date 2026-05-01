using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.StoreSettings.Repositories;

public interface IStoreSettingRepository : ITenantRepository<StoreSetting>
{
    Task<StoreSetting?> GetCurrentAsync(
        CancellationToken cancellationToken = default);
}