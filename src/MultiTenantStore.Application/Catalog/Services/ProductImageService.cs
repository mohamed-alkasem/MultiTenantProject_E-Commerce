using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Services;

public sealed class ProductImageService : IProductImageService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _imageRepository;
    private readonly ITenantUnitOfWork _unitOfWork;

    public ProductImageService(
        IProductRepository productRepository,
        IProductImageRepository imageRepository,
        ITenantUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<ProductImageDto>> CreateAsync(
        Guid productId,
        CreateProductImageDto dto,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.ExistsAsync(
            x => x.Id == productId && x.DeletedAt == null,
            cancellationToken);

        if (!productExists)
        {
            return ApiResponseDto<ProductImageDto>.Fail("Product was not found.");
        }

        if (dto.IsPrimary)
        {
            await ClearPrimaryImagesAsync(productId, cancellationToken);
        }

        var image = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ImageUrl = dto.ImageUrl,
            AltText = dto.AltText,
            SortOrder = dto.SortOrder,
            IsPrimary = dto.IsPrimary,
            CreatedAt = DateTime.UtcNow
        };

        await _imageRepository.AddAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<ProductImageDto>.Ok(
            MapToDto(image),
            "Product image created successfully.");
    }

    public async Task<ApiResponseDto<List<ProductImageDto>>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.ExistsAsync(
            x => x.Id == productId && x.DeletedAt == null,
            cancellationToken);

        if (!productExists)
        {
            return ApiResponseDto<List<ProductImageDto>>.Fail("Product was not found.");
        }

        var images = await _imageRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        return ApiResponseDto<List<ProductImageDto>>.Ok(
            images.Select(MapToDto).ToList());
    }

    public async Task<ApiResponseDto<ProductImageDto>> UpdateAsync(
        Guid productId,
        UpdateProductImageDto dto,
        CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.FirstOrDefaultAsync(
            x => x.Id == dto.Id &&
                 x.ProductId == productId &&
                 x.DeletedAt == null,
            cancellationToken);

        if (image is null)
        {
            return ApiResponseDto<ProductImageDto>.Fail("Product image was not found.");
        }

        if (dto.IsPrimary && !image.IsPrimary)
        {
            await ClearPrimaryImagesAsync(productId, cancellationToken);
        }

        image.ImageUrl = dto.ImageUrl;
        image.AltText = dto.AltText;
        image.SortOrder = dto.SortOrder;
        image.IsPrimary = dto.IsPrimary;
        image.UpdatedAt = DateTime.UtcNow;

        _imageRepository.Update(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<ProductImageDto>.Ok(
            MapToDto(image),
            "Product image updated successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.FirstOrDefaultAsync(
            x => x.Id == imageId &&
                 x.ProductId == productId &&
                 x.DeletedAt == null,
            cancellationToken);

        if (image is null)
        {
            return ApiResponseDto<bool>.Fail("Product image was not found.");
        }

        image.DeletedAt = DateTime.UtcNow;
        image.UpdatedAt = DateTime.UtcNow;

        _imageRepository.Update(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<bool>.Ok(true, "Product image deleted successfully.");
    }

    public async Task<ApiResponseDto<bool>> SetPrimaryAsync(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.FirstOrDefaultAsync(
            x => x.Id == imageId &&
                 x.ProductId == productId &&
                 x.DeletedAt == null,
            cancellationToken);

        if (image is null)
        {
            return ApiResponseDto<bool>.Fail("Product image was not found.");
        }

        await ClearPrimaryImagesAsync(productId, cancellationToken);

        image.IsPrimary = true;
        image.UpdatedAt = DateTime.UtcNow;

        _imageRepository.Update(image);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<bool>.Ok(true, "Primary image updated successfully.");
    }

    private async Task ClearPrimaryImagesAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var images = await _imageRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        foreach (var image in images.Where(x => x.IsPrimary))
        {
            image.IsPrimary = false;
            image.UpdatedAt = DateTime.UtcNow;
            _imageRepository.Update(image);
        }
    }

    private static ProductImageDto MapToDto(ProductImage image)
    {
        return new ProductImageDto
        {
            Id = image.Id,
            ProductId = image.ProductId,
            ImageUrl = image.ImageUrl,
            AltText = image.AltText,
            SortOrder = image.SortOrder,
            IsPrimary = image.IsPrimary
        };
    }
}