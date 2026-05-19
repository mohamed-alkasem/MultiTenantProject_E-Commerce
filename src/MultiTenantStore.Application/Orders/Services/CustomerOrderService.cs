using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Orders.Repositories;
using MultiTenantStore.Application.Payments.DTOs;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Orders.Services;

public sealed class CustomerOrderService : ICustomerOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentCustomerService _currentCustomerService;

    public CustomerOrderService(
        IOrderRepository orderRepository,
        ICurrentCustomerService currentCustomerService)
    {
        _orderRepository = orderRepository;
        _currentCustomerService = currentCustomerService;
    }

    public async Task<ApiResponseDto<List<OrderListDto>>> GetMyOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<List<OrderListDto>>.Fail(
                "Customer is not authenticated.");
        }

        var orders = await _orderRepository.GetByCustomerIdAsync(
            customerId.Value,
            cancellationToken);

        var result = orders
            .Select(MapToListDto)
            .ToList();

        return ApiResponseDto<List<OrderListDto>>.Ok(result);
    }

    public async Task<ApiResponseDto<OrderDto>> GetMyOrderByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<OrderDto>.Fail(
                "Customer is not authenticated.");
        }

        var order = await _orderRepository.GetCustomerOrderDetailsAsync(
            orderId,
            customerId.Value,
            cancellationToken);

        if (order is null)
        {
            return ApiResponseDto<OrderDto>.Fail("Order was not found.");
        }

        return ApiResponseDto<OrderDto>.Ok(MapToDto(order));
    }

    private static OrderListDto MapToListDto(Order order)
    {
        return new OrderListDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.ShippingFullName,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            ShippingStatus = order.ShippingStatus.ToString(),
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            CreatedAt = order.CreatedAt,
            InvoiceNumber = order.Invoice?.InvoiceNumber,
            InvoicePdfUrl = order.Invoice?.PdfUrl,
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

            CreatedAt = order.CreatedAt
        };
    }
}