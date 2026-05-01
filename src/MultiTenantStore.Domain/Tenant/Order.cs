using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Tenant;

public class Order : AuditableEntity, ISoftDelete
{
    public string OrderNumber { get; set; } = default!;

    public Guid? CustomerId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public ShippingStatus ShippingStatus { get; set; } = ShippingStatus.NotShipped;

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = default!;

    public string ShippingFullName { get; set; } = default!;
    public string ShippingPhone { get; set; } = default!;
    public string ShippingCountry { get; set; } = default!;
    public string ShippingCity { get; set; } = default!;
    public string? ShippingDistrict { get; set; }
    public string ShippingAddressLine1 { get; set; } = default!;
    public string? ShippingAddressLine2 { get; set; }
    public string? ShippingPostalCode { get; set; }

    public string BillingFullName { get; set; } = default!;
    public string BillingPhone { get; set; } = default!;
    public string BillingCountry { get; set; } = default!;
    public string BillingCity { get; set; } = default!;
    public string? BillingDistrict { get; set; }
    public string BillingAddressLine1 { get; set; } = default!;
    public string? BillingAddressLine2 { get; set; }
    public string? BillingPostalCode { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Customer? Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public Invoice? Invoice { get; set; }
}