using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Services;

public sealed class ProductVariantService : IProductVariantService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _variantRepository;
    private readonly ITenantUnitOfWork _unitOfWork;

    public ProductVariantService(
        IProductRepository productRepository,
        IProductVariantRepository variantRepository,
        ITenantUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<ProductVariantDto>> CreateAsync(
        Guid productId,
        CreateProductVariantDto dto,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.ExistsAsync(
            x => x.Id == productId && x.DeletedAt == null,
            cancellationToken);

        if (!productExists)
        {
            return ApiResponseDto<ProductVariantDto>.Fail("Product was not found.");
        }

        var sku = NormalizeSku(dto.SKU);

        var skuExists = await _variantRepository.ExistsBySkuAsync(
            sku,
            cancellationToken: cancellationToken);

        if (skuExists)
        {
            return ApiResponseDto<ProductVariantDto>.Fail("Variant SKU is already used.");
        }

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            SKU = sku,
            Name = dto.Name.Trim(),
            Price = dto.Price,
            CompareAtPrice = dto.CompareAtPrice,
            CostPrice = dto.CostPrice,
            StockQuantity = dto.StockQuantity,
            TrackInventory = dto.TrackInventory,
            AttributesJson = dto.AttributesJson,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _variantRepository.AddAsync(variant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<ProductVariantDto>.Ok(
            MapToDto(variant),
            "Product variant created successfully.");
    }

    public async Task<ApiResponseDto<List<ProductVariantDto>>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var productExists = await _productRepository.ExistsAsync(
            x => x.Id == productId && x.DeletedAt == null,
            cancellationToken);

        if (!productExists)
        {
            return ApiResponseDto<List<ProductVariantDto>>.Fail("Product was not found.");
        }

        var variants = await _variantRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        return ApiResponseDto<List<ProductVariantDto>>.Ok(
            variants.Select(MapToDto).ToList());
    }

    public async Task<ApiResponseDto<ProductVariantDto>> UpdateAsync(
        Guid productId,
        UpdateProductVariantDto dto,
        CancellationToken cancellationToken = default)
    {
        var variant = await _variantRepository.FirstOrDefaultAsync(
            x => x.Id == dto.Id &&
                 x.ProductId == productId &&
                 x.DeletedAt == null,
            cancellationToken);

        if (variant is null)
        {
            return ApiResponseDto<ProductVariantDto>.Fail("Product variant was not found.");
        }

        var sku = NormalizeSku(dto.SKU);

        var skuExists = await _variantRepository.ExistsBySkuAsync(
            sku,
            dto.Id,
            cancellationToken);

        if (skuExists)
        {
            return ApiResponseDto<ProductVariantDto>.Fail("Variant SKU is already used.");
        }

        variant.SKU = sku;
        variant.Name = dto.Name.Trim();
        variant.Price = dto.Price;
        variant.CompareAtPrice = dto.CompareAtPrice;
        variant.CostPrice = dto.CostPrice;
        variant.StockQuantity = dto.StockQuantity;
        variant.TrackInventory = dto.TrackInventory;
        variant.AttributesJson = dto.AttributesJson;
        variant.IsActive = dto.IsActive;
        variant.UpdatedAt = DateTime.UtcNow;

        _variantRepository.Update(variant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<ProductVariantDto>.Ok(
            MapToDto(variant),
            "Product variant updated successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken = default)
    {
        var variant = await _variantRepository.FirstOrDefaultAsync(
            x => x.Id == variantId &&
                 x.ProductId == productId &&
                 x.DeletedAt == null,
            cancellationToken);

        if (variant is null)
        {
            return ApiResponseDto<bool>.Fail("Product variant was not found.");
        }

        variant.DeletedAt = DateTime.UtcNow;
        variant.UpdatedAt = DateTime.UtcNow;

        _variantRepository.Update(variant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<bool>.Ok(true, "Product variant deleted successfully.");
    }

    private static string NormalizeSku(string sku)
    {
        return sku.Trim().ToUpperInvariant();
    }

    private static ProductVariantDto MapToDto(ProductVariant variant)
    {
        return new ProductVariantDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            SKU = variant.SKU,
            Name = variant.Name,
            Price = variant.Price,
            CompareAtPrice = variant.CompareAtPrice,
            CostPrice = variant.CostPrice,
            StockQuantity = variant.StockQuantity,
            TrackInventory = variant.TrackInventory,
            AttributesJson = variant.AttributesJson,
            IsActive = variant.IsActive
        };
    }
}