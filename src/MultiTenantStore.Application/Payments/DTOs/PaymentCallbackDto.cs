namespace MultiTenantStore.Application.Payments.DTOs;

public sealed class PaymentCallbackDto
{
    public required string PaymentProvider { get; init; }
    public required string ProviderReference { get; init; }
    public required string TransactionId { get; init; }
    public required string Status { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public string? FailureReason { get; init; }
}
