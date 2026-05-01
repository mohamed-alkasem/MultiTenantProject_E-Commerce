using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Catalog.Services;

public interface ICategoryService
{
    Task<ApiResponseDto<CategoryDto>> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<List<CategoryDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CategoryDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CategoryDto>> UpdateAsync(
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}