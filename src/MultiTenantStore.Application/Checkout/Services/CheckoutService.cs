using MultiTenantStore.Application.Carts.Repositories;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Orders.Repositories;
using MultiTenantStore.Application.Payments.DTOs;
using MultiTenantStore.Application.Payments.Repositories;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Checkout.Services;

public sealed class CheckoutService : ICheckoutService
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _variantRepository;
    private readonly ITenantUnitOfWork _unitOfWork;
    private readonly ICurrentCustomerService _currentCustomerService;

    public CheckoutService(
        ICartRepository cartRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IPaymentRepository paymentRepository,
        IProductRepository productRepository,
        IProductVariantRepository variantRepository,
        ITenantUnitOfWork unitOfWork,
        ICurrentCustomerService currentCustomerService)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _paymentRepository = paymentRepository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _unitOfWork = unitOfWork;
        _currentCustomerService = currentCustomerService;
    }

    public async Task<ApiResponseDto<OrderDto>> CreateOrderAsync(
        CreateOrderDto dto,
        CancellationToken cancellationToken = default)
    {
        var hasCart = dto.CartId is not null;
        var hasItems = dto.Items is not null && dto.Items.Count > 0;

        if (!hasCart && !hasItems)
        {
            return ApiResponseDto<OrderDto>.Fail("Order must contain cart or items.");
        }

        var currentCustomerId = _currentCustomerService.CustomerId;

        // إذا الزبون عامل login، لا نثق بـ customerId القادم من body.
        // إذا guest، يبقى null أو القيمة القادمة من dto حسب الحاجة.
        var orderCustomerId = currentCustomerId ?? dto.CustomerId;

        var orderLinesResult = hasCart
            ? await BuildLinesFromCartAsync(dto.CartId!.Value, cancellationToken)
            : await BuildLinesFromItemsAsync(dto.Items, cancellationToken);

        if (!orderLinesResult.Success || orderLinesResult.Data is null)
        {
            return ApiResponseDto<OrderDto>.Fail(
                orderLinesResult.Message ?? "Could not build order items.",
                orderLinesResult.Errors);
        }

        var orderLines = orderLinesResult.Data;

        if (orderLines.Count == 0)
        {
            return ApiResponseDto<OrderDto>.Fail("Order has no valid items.");
        }

        var subtotal = orderLines.Sum(x => x.LineTotal);
        var discountAmount = 0m;
        var shippingAmount = 0m;
        var taxAmount = 0m;
        var totalAmount = subtotal - discountAmount + shippingAmount + taxAmount;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),

            CustomerId = orderCustomerId,

            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            ShippingStatus = ShippingStatus.NotShipped,

            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingAmount = shippingAmount,
            TaxAmount = taxAmount,
            TotalAmount = totalAmount,
            Currency = "USD",

            ShippingFullName = dto.ShippingAddress.FullName,
            ShippingPhone = dto.ShippingAddress.Phone,
            ShippingCountry = dto.ShippingAddress.Country,
            ShippingCity = dto.ShippingAddress.City,
            ShippingDistrict = dto.ShippingAddress.District,
            ShippingAddressLine1 = dto.ShippingAddress.AddressLine1,
            ShippingAddressLine2 = dto.ShippingAddress.AddressLine2,
            ShippingPostalCode = dto.ShippingAddress.PostalCode,

            BillingFullName = dto.BillingAddress.FullName,
            BillingPhone = dto.BillingAddress.Phone,
            BillingCountry = dto.BillingAddress.Country,
            BillingCity = dto.BillingAddress.City,
            BillingDistrict = dto.BillingAddress.District,
            BillingAddressLine1 = dto.BillingAddress.AddressLine1,
            BillingAddressLine2 = dto.BillingAddress.AddressLine2,
            BillingPostalCode = dto.BillingAddress.PostalCode,

            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        foreach (var line in orderLines)
        {
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = line.ProductId,
                ProductVariantId = line.ProductVariantId,
                ProductNameSnapshot = line.ProductNameSnapshot,
                ProductImageUrlSnapshot = line.ProductImageUrlSnapshot,
                VariantInfoSnapshot = line.VariantInfoSnapshot,
                SKU = line.SKU,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                LineTotal = line.LineTotal,
                CreatedAt = DateTime.UtcNow
            };

            await _orderItemRepository.AddAsync(orderItem, cancellationToken);
            order.Items.Add(orderItem);
        }

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Amount = totalAmount,
            RefundedAmount = 0,
            Currency = order.Currency,
            PaymentMethod = dto.PaymentMethod,
            PaymentProvider = "Manual",
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);
        order.Payments.Add(payment);

        await DecreaseStockAsync(orderLines, cancellationToken);

        if (dto.CartId is not null)
        {
            var cart = await _cartRepository.GetDetailsAsync(
                dto.CartId.Value,
                cancellationToken);

            if (cart is not null)
            {
                cart.Status = CartStatus.Converted;
                cart.UpdatedAt = DateTime.UtcNow;

                _cartRepository.Update(cart);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<OrderDto>.Ok(
            MapToDto(order),
            "Order created successfully.");
    }

    private async Task<ApiResponseDto<List<OrderLineBuildResult>>> BuildLinesFromCartAsync(
        Guid cartId,
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetDetailsAsync(
            cartId,
            cancellationToken);

        if (cart is null)
        {
            return ApiResponseDto<List<OrderLineBuildResult>>.Fail("Cart was not found.");
        }

        if (cart.Status != CartStatus.Active)
        {
            return ApiResponseDto<List<OrderLineBuildResult>>.Fail("Cart is not active.");
        }

        var lines = new List<OrderLineBuildResult>();

        foreach (var item in cart.Items.Where(x => x.DeletedAt == null))
        {
            var lineResult = await BuildLineAsync(
                item.ProductId,
                item.ProductVariantId,
                item.Quantity,
                cancellationToken);

            if (!lineResult.Success || lineResult.Data is null)
            {
                return ApiResponseDto<List<OrderLineBuildResult>>.Fail(
                    lineResult.Message ?? "Invalid cart item.",
                    lineResult.Errors);
            }

            lines.Add(lineResult.Data);
        }

        return ApiResponseDto<List<OrderLineBuildResult>>.Ok(lines);
    }

    private async Task<ApiResponseDto<List<OrderLineBuildResult>>> BuildLinesFromItemsAsync(
        List<CreateOrderItemDto> items,
        CancellationToken cancellationToken)
    {
        var lines = new List<OrderLineBuildResult>();

        foreach (var item in items)
        {
            var lineResult = await BuildLineAsync(
                item.ProductId,
                item.ProductVariantId,
                item.Quantity,
                cancellationToken);

            if (!lineResult.Success || lineResult.Data is null)
            {
                return ApiResponseDto<List<OrderLineBuildResult>>.Fail(
                    lineResult.Message ?? "Invalid order item.",
                    lineResult.Errors);
            }

            lines.Add(lineResult.Data);
        }

        return ApiResponseDto<List<OrderLineBuildResult>>.Ok(lines);
    }

    private async Task<ApiResponseDto<OrderLineBuildResult>> BuildLineAsync(
        Guid productId,
        Guid? productVariantId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
        {
            return ApiResponseDto<OrderLineBuildResult>.Fail("Quantity must be greater than zero.");
        }

        var product = await _productRepository.GetDetailsAsync(
            productId,
            cancellationToken);

        if (product is null || !product.IsActive || product.DeletedAt is not null)
        {
            return ApiResponseDto<OrderLineBuildResult>.Fail("Product was not found.");
        }

        ProductVariant? variant = null;

        if (productVariantId is not null)
        {
            variant = await _variantRepository.FirstOrDefaultAsync(
                x => x.Id == productVariantId.Value &&
                     x.ProductId == product.Id &&
                     x.IsActive &&
                     x.DeletedAt == null,
                cancellationToken);

            if (variant is null)
            {
                return ApiResponseDto<OrderLineBuildResult>.Fail("Product variant was not found.");
            }
        }

        var trackInventory = variant?.TrackInventory ?? product.TrackInventory;
        var stockQuantity = variant?.StockQuantity ?? product.StockQuantity;

        if (trackInventory && stockQuantity < quantity)
        {
            return ApiResponseDto<OrderLineBuildResult>.Fail(
                $"Not enough stock for product '{product.Name}'.");
        }

        var unitPrice = variant?.Price ?? product.Price;

        var primaryImageUrl = product.Images
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .Select(x => x.ImageUrl)
            .FirstOrDefault();

        return ApiResponseDto<OrderLineBuildResult>.Ok(
            new OrderLineBuildResult
            {
                ProductId = product.Id,
                ProductVariantId = variant?.Id,
                ProductNameSnapshot = product.Name,
                ProductImageUrlSnapshot = primaryImageUrl,
                VariantInfoSnapshot = variant?.Name,
                SKU = variant?.SKU ?? product.SKU,
                Quantity = quantity,
                UnitPrice = unitPrice,
                LineTotal = unitPrice * quantity
            });
    }

    private async Task DecreaseStockAsync(
        List<OrderLineBuildResult> lines,
        CancellationToken cancellationToken)
    {
        foreach (var line in lines)
        {
            if (line.ProductVariantId is not null)
            {
                var variant = await _variantRepository.FirstOrDefaultAsync(
                    x => x.Id == line.ProductVariantId.Value &&
                         x.DeletedAt == null,
                    cancellationToken);

                if (variant is not null && variant.TrackInventory)
                {
                    variant.StockQuantity -= line.Quantity;
                    variant.UpdatedAt = DateTime.UtcNow;

                    _variantRepository.Update(variant);
                }

                continue;
            }

            var product = await _productRepository.FirstOrDefaultAsync(
                x => x.Id == line.ProductId &&
                     x.DeletedAt == null,
                cancellationToken);

            if (product is not null && product.TrackInventory)
            {
                product.StockQuantity -= line.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                _productRepository.Update(product);
            }
        }
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.ShippingFullName,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            ShippingStatus = order.ShippingStatus.ToString(),
            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            ShippingAmount = order.ShippingAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            ShippingAddress = new OrderAddressDto
            {
                FullName = order.ShippingFullName,
                Phone = order.ShippingPhone,
                Country = order.ShippingCountry,
                City = order.ShippingCity,
                District = order.ShippingDistrict,
                AddressLine1 = order.ShippingAddressLine1,
                AddressLine2 = order.ShippingAddressLine2,
                PostalCode = order.ShippingPostalCode
            },
            BillingAddress = new OrderAddressDto
            {
                FullName = order.BillingFullName,
                Phone = order.BillingPhone,
                Country = order.BillingCountry,
                City = order.BillingCity,
                District = order.BillingDistrict,
                AddressLine1 = order.BillingAddressLine1,
                AddressLine2 = order.BillingAddressLine2,
                PostalCode = order.BillingPostalCode
            },
            Items = order.Items
                .Where(x => x.DeletedAt == null)
                .Select(x => new OrderItemDto
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProductNameSnapshot = x.ProductNameSnapshot,
                    ProductImageUrlSnapshot = x.ProductImageUrlSnapshot,
                    VariantInfoSnapshot = x.VariantInfoSnapshot,
                    SKU = x.SKU,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    LineTotal = x.LineTotal
                })
                .ToList(),
            Payments = order.Payments
                .Where(x => x.DeletedAt == null)
                .Select(x => new PaymentDto
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    Amount = x.Amount,
                    RefundedAmount = x.RefundedAmount,
                    Currency = x.Currency,
                    PaymentMethod = x.PaymentMethod,
                    PaymentProvider = x.PaymentProvider,
                    ProviderReference = x.ProviderReference,
                    TransactionId = x.TransactionId,
                    Status = x.Status.ToString(),
                    FailureReason = x.FailureReason,
                    ProviderResponseCode = x.ProviderResponseCode,
                    PaidAt = x.PaidAt
                })
                .ToList(),
            CreatedAt = order.CreatedAt
        };
    }

    private sealed class OrderLineBuildResult
    {
        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public string ProductNameSnapshot { get; set; } = default!;

        public string? ProductImageUrlSnapshot { get; set; }

        public string? VariantInfoSnapshot { get; set; }

        public string SKU { get; set; } = default!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}