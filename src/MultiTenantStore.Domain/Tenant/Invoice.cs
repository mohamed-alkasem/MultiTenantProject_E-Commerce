using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Tenant;

public class Invoice : AuditableEntity, ISoftDelete
{
    public Guid OrderId { get; set; }

    public string InvoiceNumber { get; set; } = default!;

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = default!;

    public string CustomerNameSnapshot { get; set; } = default!;
    public string? CustomerEmailSnapshot { get; set; }
    public string BillingAddressSnapshot { get; set; } = default!;

    public string StoreNameSnapshot { get; set; } = default!;
    public string? StoreTaxNumberSnapshot { get; set; }
    public string? StoreAddressSnapshot { get; set; }

    public string? PdfUrl { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Order Order { get; set; } = default!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}