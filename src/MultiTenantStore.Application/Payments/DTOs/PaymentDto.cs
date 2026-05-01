namespace MultiTenantStore.Application.Payments.DTOs;

public sealed class PaymentDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public decimal RefundedAmount { get; init; }
    public required string Currency { get; init; }
    public required string PaymentMethod { get; init; }
    public required string PaymentProvider { get; init; }
    public string? ProviderReference { get; init; }
    public string? TransactionId { get; init; }
    public required string Status { get; init; }
    public string? FailureReason { get; init; }
    public string? ProviderResponseCode { get; init; }
    public DateTime? PaidAt { get; init; }
}
