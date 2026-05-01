namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class CreateOrderDto
{
    public Guid? CustomerId { get; init; }
    public Guid? CartId { get; init; }
    public List<CreateOrderItemDto> Items { get; init; } = new();
    public required CreateOrderAddressDto ShippingAddress { get; init; }
    public required CreateOrderAddressDto BillingAddress { get; init; }
    public required string PaymentMethod { get; init; }
}
