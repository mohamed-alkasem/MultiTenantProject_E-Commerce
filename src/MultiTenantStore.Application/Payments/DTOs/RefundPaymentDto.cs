namespace MultiTenantStore.Application.Payments.DTOs;

public sealed class RefundPaymentDto
{
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public required string Reason { get; init; }
}
