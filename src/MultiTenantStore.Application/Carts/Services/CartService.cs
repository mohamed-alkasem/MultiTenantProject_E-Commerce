using MultiTenantStore.Application.Carts.DTOs;
using MultiTenantStore.Application.Carts.Repositories;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Carts.Services;

public sealed class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _variantRepository;
    private readonly ITenantUnitOfWork _unitOfWork;
    private readonly ICurrentCustomerService _currentCustomerService;

    public CartService(
    ICartRepository cartRepository,
    ICartItemRepository cartItemRepository,
    IProductRepository productRepository,
    IProductVariantRepository variantRepository,
    ITenantUnitOfWork unitOfWork,
    ICurrentCustomerService currentCustomerService)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _unitOfWork = unitOfWork;
        _currentCustomerService = currentCustomerService;
    }

    public async Task<ApiResponseDto<CartDto>> GetOrCreateCartAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return ApiResponseDto<CartDto>.Fail("SessionId is required.");
        }

        sessionId = sessionId.Trim();

        var cart = await _cartRepository.GetDetailsBySessionIdAsync(
            sessionId,
            cancellationToken);

        if (cart is not null)
        {
            return ApiResponseDto<CartDto>.Ok(MapToDto(cart));
        }

        cart = new Cart
        {
            Id = Guid.NewGuid(),
            CustomerId = null,
            SessionId = sessionId,
            Status = CartStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _cartRepository.AddAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CartDto>.Ok(
            MapToDto(cart),
            "Cart created successfully.");
    }

    public async Task<ApiResponseDto<CartDto>> GetCartAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return ApiResponseDto<CartDto>.Fail("SessionId is required.");
        }

        sessionId = sessionId.Trim();

        var cart = await _cartRepository.GetDetailsBySessionIdAsync(
            sessionId,
            cancellationToken);

        if (cart is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart was not found.");
        }

        return ApiResponseDto<CartDto>.Ok(MapToDto(cart));
    }

    public async Task<ApiResponseDto<CartDto>> AddItemAsync(
        string sessionId,
        AddToCartDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return ApiResponseDto<CartDto>.Fail("SessionId is required.");
        }

        if (dto.Quantity <= 0)
        {
            return ApiResponseDto<CartDto>.Fail("Quantity must be greater than zero.");
        }

        sessionId = sessionId.Trim();

        var cartResult = await GetOrCreateCartAsync(
            sessionId,
            cancellationToken);

        if (!cartResult.Success)
        {
            return ApiResponseDto<CartDto>.Fail(
                cartResult.Message ?? "Cart could not be created.",
                cartResult.Errors);
        }

        var cart = await _cartRepository.GetActiveBySessionIdAsync(
            sessionId,
            cancellationToken);

        if (cart is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart was not found.");
        }

        var product = await _productRepository.GetDetailsAsync(
            dto.ProductId,
            cancellationToken);

        if (product is null || !product.IsActive || product.DeletedAt is not null)
        {
            return ApiResponseDto<CartDto>.Fail("Product was not found.");
        }

        ProductVariant? variant = null;

        if (dto.ProductVariantId is not null)
        {
            variant = await _variantRepository.FirstOrDefaultAsync(
                x => x.Id == dto.ProductVariantId.Value &&
                     x.ProductId == product.Id &&
                     x.IsActive &&
                     x.DeletedAt == null,
                cancellationToken);

            if (variant is null)
            {
                return ApiResponseDto<CartDto>.Fail("Product variant was not found.");
            }
        }

        var stockQuantity = variant?.StockQuantity ?? product.StockQuantity;

        if (stockQuantity < dto.Quantity)
        {
            return ApiResponseDto<CartDto>.Fail("Not enough stock.");
        }

        var unitPrice = variant?.Price ?? product.Price;

        var existingItem = await _cartItemRepository.GetByCartAndProductAsync(
            cart.Id,
            product.Id,
            variant?.Id,
            cancellationToken);

        if (existingItem is not null)
        {
            var newQuantity = existingItem.Quantity + dto.Quantity;

            if (stockQuantity < newQuantity)
            {
                return ApiResponseDto<CartDto>.Fail("Not enough stock.");
            }

            existingItem.Quantity = newQuantity;
            existingItem.UnitPrice = unitPrice;
            existingItem.UpdatedAt = DateTime.UtcNow;

            _cartItemRepository.Update(existingItem);
        }
        else
        {
            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = product.Id,
                ProductVariantId = variant?.Id,
                Quantity = dto.Quantity,
                UnitPrice = unitPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _cartItemRepository.AddAsync(cartItem, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedCart = await _cartRepository.GetDetailsAsync(
            cart.Id,
            cancellationToken);

        if (updatedCart is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart could not be loaded after update.");
        }

        return ApiResponseDto<CartDto>.Ok(
            MapToDto(updatedCart),
            "Item added to cart.");
    }

    public async Task<ApiResponseDto<CartDto>> UpdateItemQuantityAsync(
        string sessionId,
        UpdateCartItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return ApiResponseDto<CartDto>.Fail("SessionId is required.");
        }

        if (dto.Quantity <= 0)
        {
            return ApiResponseDto<CartDto>.Fail("Quantity must be greater than zero.");
        }

        sessionId = sessionId.Trim();

        var cart = await _cartRepository.GetActiveBySessionIdAsync(
            sessionId,
            cancellationToken);

        if (cart is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart was not found.");
        }

        var item = await _cartItemRepository.GetDetailsAsync(
            dto.CartItemId,
            cancellationToken);

        if (item is null || item.CartId != cart.Id)
        {
            return ApiResponseDto<CartDto>.Fail("Cart item was not found.");
        }

        var stockQuantity = item.ProductVariant?.StockQuantity ?? item.Product.StockQuantity;

        if (stockQuantity < dto.Quantity)
        {
            return ApiResponseDto<CartDto>.Fail("Not enough stock.");
        }

        var currentUnitPrice = item.ProductVariant?.Price ?? item.Product.Price;

        item.Quantity = dto.Quantity;
        item.UnitPrice = currentUnitPrice;
        item.UpdatedAt = DateTime.UtcNow;

        _cartItemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedCart = await _cartRepository.GetDetailsAsync(
            cart.Id,
            cancellationToken);

        if (updatedCart is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart could not be loaded after update.");
        }

        return ApiResponseDto<CartDto>.Ok(
            MapToDto(updatedCart),
            "Cart item updated successfully.");
    }

    public async Task<ApiResponseDto<CartDto>> RemoveItemAsync(
        string sessionId,
        RemoveCartItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return ApiResponseDto<CartDto>.Fail("SessionId is required.");
        }

        sessionId = sessionId.Trim();

        var cart = await _cartRepository.GetActiveBySessionIdAsync(
            sessionId,
            cancellationToken);

        if (cart is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart was not found.");
        }

        var item = await _cartItemRepository.FirstOrDefaultAsync(
            x => x.Id == dto.CartItemId &&
                 x.CartId == cart.Id &&
                 x.DeletedAt == null,
            cancellationToken);

        if (item is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart item was not found.");
        }

        item.DeletedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        _cartItemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedCart = await _cartRepository.GetDetailsAsync(
            cart.Id,
            cancellationToken);

        if (updatedCart is null)
        {
            return ApiResponseDto<CartDto>.Fail("Cart could not be loaded after update.");
        }

        return ApiResponseDto<CartDto>.Ok(
            MapToDto(updatedCart),
            "Cart item removed successfully.");
    }

    public async Task<ApiResponseDto<bool>> ClearCartAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return ApiResponseDto<bool>.Fail("SessionId is required.");
        }

        sessionId = sessionId.Trim();

        var cart = await _cartRepository.GetActiveBySessionIdAsync(
            sessionId,
            cancellationToken);

        if (cart is null)
        {
            return ApiResponseDto<bool>.Fail("Cart was not found.");
        }

        var items = await _cartItemRepository.GetByCartIdAsync(
            cart.Id,
            cancellationToken);

        foreach (var item in items)
        {
            item.DeletedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;

            _cartItemRepository.Update(item);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<bool>.Ok(true, "Cart cleared successfully.");
    }

    private static CartDto MapToDto(Cart cart)
    {
        var items = cart.Items
            .Where(x => x.DeletedAt == null)
            .Select(x =>
            {
                var product = x.Product;
                var variant = x.ProductVariant;

                return new CartItemDto
                {
                    Id = x.Id,
                    CartId = x.CartId,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProductName = product?.Name ?? string.Empty,
                    ProductImageUrl = product?.Images?
                        .Where(i => i.DeletedAt == null)
                        .OrderByDescending(i => i.IsPrimary)
                        .ThenBy(i => i.SortOrder)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    VariantName = variant?.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    LineTotal = x.UnitPrice * x.Quantity
                };
            })
            .ToList();

        var subtotal = items.Sum(x => x.LineTotal);

        return new CartDto
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            SessionId = cart.SessionId,
            Status = cart.Status.ToString(),
            Subtotal = subtotal,
            TotalAmount = subtotal,
            Items = items
        };
    }

    public async Task<ApiResponseDto<CartDto>> GetCustomerCartAsync(
    CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<CartDto>.Fail("Customer is not authenticated.");
        }

        var cart = await _cartRepository.GetDetailsByCustomerIdAsync(
            customerId.Value,
            cancellationToken);

        if (cart is not null)
        {
            return ApiResponseDto<CartDto>.Ok(MapToDto(cart));
        }

        cart = new Cart
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId.Value,
            SessionId = null,
            Status = CartStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _cartRepository.AddAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CartDto>.Ok(
            MapToDto(cart),
            "Customer cart created successfully.");
    }

    public async Task<ApiResponseDto<CartDto>> MergeGuestCartAsync(
        MergeCartDto dto,
        CancellationToken cancellationToken = default)
    {
        var currentCustomerId = _currentCustomerService.CustomerId;

        if (currentCustomerId is null)
        {
            return ApiResponseDto<CartDto>.Fail("Customer is not authenticated.");
        }

        if (dto.CustomerId != currentCustomerId.Value)
        {
            return ApiResponseDto<CartDto>.Fail("You cannot merge cart for another customer.");
        }

        if (string.IsNullOrWhiteSpace(dto.SessionId))
        {
            return ApiResponseDto<CartDto>.Fail("SessionId is required.");
        }

        var sessionId = dto.SessionId.Trim();

        var guestCart = await _cartRepository.GetDetailsBySessionIdAsync(
            sessionId,
            cancellationToken);

        if (guestCart is null)
        {
            return await GetCustomerCartAsync(cancellationToken);
        }

        var customerCart = await _cartRepository.GetDetailsByCustomerIdAsync(
            currentCustomerId.Value,
            cancellationToken);

        if (customerCart is null)
        {
            guestCart.CustomerId = currentCustomerId.Value;
            guestCart.SessionId = null;
            guestCart.UpdatedAt = DateTime.UtcNow;

            _cartRepository.Update(guestCart);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedCart = await _cartRepository.GetDetailsAsync(
                guestCart.Id,
                cancellationToken);

            return ApiResponseDto<CartDto>.Ok(
                MapToDto(updatedCart!),
                "Guest cart assigned to customer.");
        }

        foreach (var guestItem in guestCart.Items.Where(x => x.DeletedAt == null))
        {
            var existingCustomerItem = customerCart.Items.FirstOrDefault(x =>
                x.DeletedAt == null &&
                x.ProductId == guestItem.ProductId &&
                x.ProductVariantId == guestItem.ProductVariantId);

            if (existingCustomerItem is null)
            {
                guestItem.CartId = customerCart.Id;
                guestItem.UpdatedAt = DateTime.UtcNow;

                _cartItemRepository.Update(guestItem);
                continue;
            }

            existingCustomerItem.Quantity += guestItem.Quantity;
            existingCustomerItem.UpdatedAt = DateTime.UtcNow;

            _cartItemRepository.Update(existingCustomerItem);

            guestItem.DeletedAt = DateTime.UtcNow;
            guestItem.UpdatedAt = DateTime.UtcNow;

            _cartItemRepository.Update(guestItem);
        }

        guestCart.Status = CartStatus.Converted;
        guestCart.UpdatedAt = DateTime.UtcNow;

        _cartRepository.Update(guestCart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var mergedCart = await _cartRepository.GetDetailsAsync(
            customerCart.Id,
            cancellationToken);

        return ApiResponseDto<CartDto>.Ok(
            MapToDto(mergedCart!),
            "Guest cart merged with customer cart.");
    }
}