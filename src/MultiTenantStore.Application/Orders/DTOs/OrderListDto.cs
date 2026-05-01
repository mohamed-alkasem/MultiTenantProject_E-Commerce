namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class OrderListDto
{
    public Guid Id { get; init; }
    public required string OrderNumber { get; init; }
    public string? CustomerName { get; init; }
    public required string Status { get; init; }
    public required string PaymentStatus { get; init; }
    public required string ShippingStatus { get; init; }
    public decimal TotalAmount { get; init; }
    public required string Currency { get; init; }
    public DateTime CreatedAt { get; init; }
}
