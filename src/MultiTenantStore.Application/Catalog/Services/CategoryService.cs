using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITenantUnitOfWork _unitOfWork;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ITenantUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<CategoryDto>> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var slug = NormalizeSlug(dto.Slug);

        var slugExists = await _categoryRepository.ExistsBySlugAsync(
            slug,
            cancellationToken: cancellationToken);

        if (slugExists)
        {
            return ApiResponseDto<CategoryDto>.Fail("Category slug is already used.");
        }

        if (dto.ParentCategoryId is not null)
        {
            var parentExists = await _categoryRepository.ExistsAsync(
                x => x.Id == dto.ParentCategoryId.Value && x.DeletedAt == null,
                cancellationToken);

            if (!parentExists)
            {
                return ApiResponseDto<CategoryDto>.Fail("Parent category was not found.");
            }
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            ParentCategoryId = dto.ParentCategoryId,
            Name = dto.Name.Trim(),
            NameAr = dto.NameAr?.Trim(),
            Slug = slug,
            IsActive = dto.IsActive,
            SortOrder = dto.SortOrder,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CategoryDto>.Ok(
            MapToDto(category),
            "Category created successfully.");
    }

    public async Task<ApiResponseDto<List<CategoryDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllNotDeletedAsync(cancellationToken);

        var result = categories
            .Select(MapToDto)
            .ToList();

        return ApiResponseDto<List<CategoryDto>>.Ok(result);
    }

    public async Task<ApiResponseDto<CategoryDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetNotDeletedByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return ApiResponseDto<CategoryDto>.Fail("Category was not found.");
        }

        return ApiResponseDto<CategoryDto>.Ok(MapToDto(category));
    }

    public async Task<ApiResponseDto<CategoryDto>> UpdateAsync(
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetNotDeletedByIdAsync(
            dto.Id,
            cancellationToken);

        if (category is null)
        {
            return ApiResponseDto<CategoryDto>.Fail("Category was not found.");
        }

        if (dto.ParentCategoryId == dto.Id)
        {
            return ApiResponseDto<CategoryDto>.Fail("Category cannot be parent of itself.");
        }

        if (dto.ParentCategoryId is not null)
        {
            var parentExists = await _categoryRepository.ExistsAsync(
                x => x.Id == dto.ParentCategoryId.Value && x.DeletedAt == null,
                cancellationToken);

            if (!parentExists)
            {
                return ApiResponseDto<CategoryDto>.Fail("Parent category was not found.");
            }
        }

        var slug = NormalizeSlug(dto.Slug);

        var slugExists = await _categoryRepository.ExistsBySlugAsync(
            slug,
            dto.Id,
            cancellationToken);

        if (slugExists)
        {
            return ApiResponseDto<CategoryDto>.Fail("Category slug is already used.");
        }

        category.ParentCategoryId = dto.ParentCategoryId;
        category.Name = dto.Name.Trim();
        category.NameAr = dto.NameAr?.Trim();
        category.Slug = slug;
        category.IsActive = dto.IsActive;
        category.SortOrder = dto.SortOrder;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CategoryDto>.Ok(
            MapToDto(category),
            "Category updated successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetNotDeletedByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return ApiResponseDto<bool>.Fail("Category was not found.");
        }

        category.DeletedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<bool>.Ok(true, "Category deleted successfully.");
    }

    private static string NormalizeSlug(string slug)
    {
        return slug.Trim().ToLowerInvariant();
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            ParentCategoryId = category.ParentCategoryId,
            Name = category.Name,
            NameAr = category.NameAr,
            Slug = category.Slug,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder
        };
    }
}