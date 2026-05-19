using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Orders.Repositories;
using MultiTenantStore.Application.Payments.DTOs;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Orders.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITenantUnitOfWork _unitOfWork;

    public OrderService(
        IOrderRepository orderRepository,
        ITenantUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<PagedResultDto<OrderListDto>>> GetOrdersAsync(
        OrderSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var (orders, totalCount) = await _orderRepository.SearchAsync(
            request,
            cancellationToken);

        var items = orders
            .Select(MapToListDto)
            .ToList();

        var result = PagedResultDto<OrderListDto>.Create(
            items,
            pageNumber,
            pageSize,
            totalCount);

        return ApiResponseDto<PagedResultDto<OrderListDto>>.Ok(result);
    }

    public async Task<ApiResponseDto<OrderDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetDetailsAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return ApiResponseDto<OrderDto>.Fail("Order was not found.");
        }

        return ApiResponseDto<OrderDto>.Ok(MapToDto(order));
    }

    public async Task<ApiResponseDto<OrderDto>> UpdateOrderStatusAsync(
        UpdateOrderStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetDetailsAsync(
            dto.OrderId,
            cancellationToken);

        if (order is null)
        {
            return ApiResponseDto<OrderDto>.Fail("Order was not found.");
        }

        if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
        {
            return ApiResponseDto<OrderDto>.Fail("Invalid order status.");
        }

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<OrderDto>.Ok(
            MapToDto(order),
            "Order status updated successfully.");
    }

    public async Task<ApiResponseDto<OrderDto>> UpdatePaymentStatusAsync(
        UpdatePaymentStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetDetailsAsync(
            dto.OrderId,
            cancellationToken);

        if (order is null)
        {
            return ApiResponseDto<OrderDto>.Fail("Order was not found.");
        }

        if (!Enum.TryParse<PaymentStatus>(dto.PaymentStatus, true, out var paymentStatus))
        {
            return ApiResponseDto<OrderDto>.Fail("Invalid payment status.");
        }

        order.PaymentStatus = paymentStatus;
        order.UpdatedAt = DateTime.UtcNow;

        foreach (var payment in order.Payments.Where(x => x.DeletedAt == null))
        {
            payment.Status = paymentStatus;
            payment.UpdatedAt = DateTime.UtcNow;

            if (paymentStatus.ToString().Equals("Paid", StringComparison.OrdinalIgnoreCase))
            {
                payment.PaidAt ??= DateTime.UtcNow;
            }
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<OrderDto>.Ok(
            MapToDto(order),
            "Payment status updated successfully.");
    }

    public async Task<ApiResponseDto<OrderDto>> UpdateShippingStatusAsync(
        UpdateShippingStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetDetailsAsync(
            dto.OrderId,
            cancellationToken);

        if (order is null)
        {
            return ApiResponseDto<OrderDto>.Fail("Order was not found.");
        }

        if (!Enum.TryParse<ShippingStatus>(dto.ShippingStatus, true, out var shippingStatus))
        {
            return ApiResponseDto<OrderDto>.Fail("Invalid shipping status.");
        }

        order.ShippingStatus = shippingStatus;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<OrderDto>.Ok(
            MapToDto(order),
            "Shipping status updated successfully.");
    }

    private static OrderListDto MapToListDto(Order order)
    {
        return new OrderListDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerId is null ? order.ShippingFullName : order.ShippingFullName,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            ShippingStatus = order.ShippingStatus.ToString(),
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            CreatedAt = order.CreatedAt
        };
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
            CreatedAt = order.CreatedAt,
            InvoiceId = order.Invoice?.Id,
            InvoiceNumber = order.Invoice?.InvoiceNumber,
            InvoicePdfUrl = order.Invoice?.PdfUrl,
        };
    }
}