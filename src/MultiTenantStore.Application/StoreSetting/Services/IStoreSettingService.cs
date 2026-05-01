using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.StoreSettings.DTOs;

namespace MultiTenantStore.Application.StoreSettings.Services;

public interface IStoreSettingService
{
    Task<ApiResponseDto<StoreSettingDto>> GetSettingsAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<StoreSettingDto>> UpdateSettingsAsync(
        UpdateStoreSettingDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<StoreSettingDto>> EnableCheckoutAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<StoreSettingDto>> DisableCheckoutAsync(
        CancellationToken cancellationToken = default);
}