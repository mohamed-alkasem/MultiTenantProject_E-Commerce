namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class CreateOrderItemDto
{
    public Guid ProductId { get; init; }
    public Guid? ProductVariantId { get; init; }
    public int Quantity { get; init; }
}
