namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class UpdateOrderStatusDto
{
    public Guid OrderId { get; init; }
    public required string Status { get; init; }
}
