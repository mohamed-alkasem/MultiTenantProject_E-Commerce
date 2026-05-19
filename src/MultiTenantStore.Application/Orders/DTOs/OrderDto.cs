using MultiTenantStore.Application.Payments.DTOs;

namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class OrderDto
{
    public Guid Id { get; init; }
    public required string OrderNumber { get; init; }
    public Guid? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public required string Status { get; init; }
    public required string PaymentStatus { get; init; }
    public required string ShippingStatus { get; init; }
    public decimal Subtotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal ShippingAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public required string Currency { get; init; }
    public required OrderAddressDto ShippingAddress { get; init; }
    public required OrderAddressDto BillingAddress { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public List<PaymentDto> Payments { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public Guid? InvoiceId { get; init; }
    public string? InvoiceNumber { get; init; }
    public string? InvoicePdfUrl { get; init; }
}
