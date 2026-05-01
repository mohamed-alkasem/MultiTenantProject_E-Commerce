namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class SubscriptionPaymentDto
{
    public Guid Id { get; set; }

    public Guid StoreSubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = default!;

    public string PaymentProvider { get; set; } = default!;

    public string? ProviderReference { get; set; }

    public string? TransactionId { get; set; }

    public string? InvoiceNumber { get; set; }

    public string PaymentStatus { get; set; } = default!;

    public string? FailureReason { get; set; }

    public DateTime? PaidAt { get; set; }
}