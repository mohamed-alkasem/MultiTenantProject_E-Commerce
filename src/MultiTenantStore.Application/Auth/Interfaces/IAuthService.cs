using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<ApiResponseDto<AuthResponseDto>> RegisterMerchantAsync(
        RegisterMerchantDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<AuthResponseDto>> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default);
}