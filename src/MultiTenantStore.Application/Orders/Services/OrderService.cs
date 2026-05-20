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
            return ApiResponseDto<OrderDto>.Fail("الطلب غير موجود.");

        if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.Refunded)
            return ApiResponseDto<OrderDto>.Fail("لا يمكن تغيير حالة طلب مكتمل أو ملغى أو مُسترد.");

        if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
            return ApiResponseDto<OrderDto>.Fail("حالة الطلب غير صحيحة.");

        // Enforce forward-only transitions
        bool validTransition = (order.Status, newStatus) switch
        {
            (OrderStatus.Pending,    OrderStatus.Confirmed)  => true,
            (OrderStatus.Pending,    OrderStatus.Cancelled)  => true,
            (OrderStatus.Confirmed,  OrderStatus.Processing) => true,
            (OrderStatus.Confirmed,  OrderStatus.Cancelled)  => true,
            (OrderStatus.Processing, OrderStatus.Completed)  => true,
            (OrderStatus.Processing, OrderStatus.Cancelled)  => true,
            _ => false
        };

        if (!validTransition)
            return ApiResponseDto<OrderDto>.Fail($"لا يمكن الانتقال من '{order.Status}' إلى '{newStatus}'.");

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<OrderDto>.Ok(MapToDto(order), "تم تحديث حالة الطلب بنجاح.");
    }

    public async Task<ApiResponseDto<OrderDto>> UpdatePaymentStatusAsync(
        UpdatePaymentStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetDetailsAsync(
            dto.OrderId,
            cancellationToken);

        if (order is null)
            return ApiResponseDto<OrderDto>.Fail("الطلب غير موجود.");

        if (order.PaymentStatus is PaymentStatus.Refunded or PaymentStatus.PartiallyRefunded)
            return ApiResponseDto<OrderDto>.Fail("لا يمكن تغيير حالة دفع مُسترد.");

        if (!Enum.TryParse<PaymentStatus>(dto.PaymentStatus, true, out var newStatus))
            return ApiResponseDto<OrderDto>.Fail("حالة الدفع غير صحيحة.");

        // Enforce valid payment transitions
        bool validTransition = (order.PaymentStatus, newStatus) switch
        {
            (PaymentStatus.Pending, PaymentStatus.Paid)              => true,
            (PaymentStatus.Pending, PaymentStatus.Failed)            => true,
            (PaymentStatus.Failed,  PaymentStatus.Pending)           => true,
            (PaymentStatus.Failed,  PaymentStatus.Paid)              => true,
            (PaymentStatus.Paid,    PaymentStatus.Refunded)          => true,
            (PaymentStatus.Paid,    PaymentStatus.PartiallyRefunded) => true,
            _ => false
        };

        if (!validTransition)
            return ApiResponseDto<OrderDto>.Fail($"لا يمكن الانتقال من '{order.PaymentStatus}' إلى '{newStatus}'.");

        order.PaymentStatus = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        foreach (var payment in order.Payments.Where(x => x.DeletedAt == null))
        {
            payment.Status = newStatus;
            payment.UpdatedAt = DateTime.UtcNow;

            if (newStatus == PaymentStatus.Paid)
                payment.PaidAt ??= DateTime.UtcNow;
        }

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<OrderDto>.Ok(MapToDto(order), "تم تحديث حالة الدفع بنجاح.");
    }

    public async Task<ApiResponseDto<OrderDto>> UpdateShippingStatusAsync(
        UpdateShippingStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetDetailsAsync(
            dto.OrderId,
            cancellationToken);

        if (order is null)
            return ApiResponseDto<OrderDto>.Fail("الطلب غير موجود.");

        if (order.ShippingStatus is ShippingStatus.Delivered or ShippingStatus.Returned)
            return ApiResponseDto<OrderDto>.Fail("لا يمكن تغيير حالة شحنة تم توصيلها أو إعادتها.");

        if (!Enum.TryParse<ShippingStatus>(dto.ShippingStatus, true, out var newStatus))
            return ApiResponseDto<OrderDto>.Fail("حالة الشحن غير صحيحة.");

        // Enforce valid shipping transitions
        bool validTransition = (order.ShippingStatus, newStatus) switch
        {
            (ShippingStatus.NotShipped, ShippingStatus.Processing)  => true,
            (ShippingStatus.NotShipped, ShippingStatus.Cancelled)   => true,
            (ShippingStatus.Processing, ShippingStatus.Shipped)     => true,
            (ShippingStatus.Processing, ShippingStatus.Cancelled)   => true,
            (ShippingStatus.Shipped,    ShippingStatus.Delivered)   => true,
            (ShippingStatus.Shipped,    ShippingStatus.Returned)    => true,
            (ShippingStatus.Cancelled,  ShippingStatus.NotShipped)  => true,
            _ => false
        };

        if (!validTransition)
            return ApiResponseDto<OrderDto>.Fail($"لا يمكن الانتقال من '{order.ShippingStatus}' إلى '{newStatus}'.");

        order.ShippingStatus = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<OrderDto>.Ok(MapToDto(order), "تم تحديث حالة الشحن بنجاح.");
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