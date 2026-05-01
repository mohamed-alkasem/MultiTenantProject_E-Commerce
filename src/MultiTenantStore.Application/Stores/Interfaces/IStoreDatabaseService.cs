using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Stores.DTOs;

namespace MultiTenantStore.Application.Stores.Interfaces;

public interface IStoreDatabaseService
{
    Task<ApiResponseDto<StoreDatabaseDto>> ProvisionDatabaseAsync(
        Guid storeId,
        CancellationToken cancellationToken = default);
}