namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class UpdatePaymentStatusDto
{
    public Guid OrderId { get; init; }
    public required string PaymentStatus { get; init; }
}
