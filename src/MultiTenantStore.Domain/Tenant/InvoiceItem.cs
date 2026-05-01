using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class InvoiceItem : AuditableEntity, ISoftDelete
{
    public Guid InvoiceId { get; set; }
    public Guid? OrderItemId { get; set; }

    public string ProductNameSnapshot { get; set; } = default!;
    public string SKU { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Invoice Invoice { get; set; } = default!;
    public OrderItem? OrderItem { get; set; }
}