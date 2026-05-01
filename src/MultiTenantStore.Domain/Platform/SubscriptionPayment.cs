using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Platform;

public class SubscriptionPayment : AuditableEntity, ISoftDelete
{
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
    public DateTime? DeletedAt { get; set; }

    public StoreSubscription StoreSubscription { get; set; } = default!;
}