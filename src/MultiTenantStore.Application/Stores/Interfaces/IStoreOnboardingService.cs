using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Stores.DTOs;

namespace MultiTenantStore.Application.Stores.Interfaces;

public interface IStoreOnboardingService
{
    Task<ApiResponseDto<StoreOnboardingResultDto>> OnboardStoreForMerchantAsync(
        CreateStoreDto dto,
        CancellationToken cancellationToken = default);
}