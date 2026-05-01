using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Catalog.Services;

public interface IProductImageService
{
    Task<ApiResponseDto<ProductImageDto>> CreateAsync(
        Guid productId,
        CreateProductImageDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<List<ProductImageDto>>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<ProductImageDto>> UpdateAsync(
        Guid productId,
        UpdateProductImageDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> DeleteAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> SetPrimaryAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken = default);
}