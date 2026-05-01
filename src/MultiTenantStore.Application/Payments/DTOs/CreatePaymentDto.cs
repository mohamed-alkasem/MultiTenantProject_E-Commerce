namespace MultiTenantStore.Application.Payments.DTOs;

public sealed class CreatePaymentDto
{
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string PaymentMethod { get; init; }
    public required string PaymentProvider { get; init; }
}
