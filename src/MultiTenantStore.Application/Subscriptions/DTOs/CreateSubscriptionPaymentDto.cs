namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class CreateSubscriptionPaymentDto
{
    public Guid StoreSubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = default!;

    public string PaymentProvider { get; set; } = default!;

    public string? ProviderReference { get; set; }

    public string? TransactionId { get; set; }
}