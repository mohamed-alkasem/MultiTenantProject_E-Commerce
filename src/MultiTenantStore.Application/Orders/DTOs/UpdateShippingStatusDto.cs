namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class UpdateShippingStatusDto
{
    public Guid OrderId { get; init; }
    public required string ShippingStatus { get; init; }
}
