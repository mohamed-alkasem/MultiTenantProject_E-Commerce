using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Tenant;

public class Payment : AuditableEntity, ISoftDelete
{
    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }
    public decimal RefundedAmount { get; set; }

    public string Currency { get; set; } = default!;

    public string PaymentMethod { get; set; } = default!;
    public string PaymentProvider { get; set; } = default!;

    public string? ProviderReference { get; set; }
    public string? TransactionId { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? FailureReason { get; set; }
    public string? ProviderResponseCode { get; set; }

    public DateTime? PaidAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Order Order { get; set; } = default!;
}