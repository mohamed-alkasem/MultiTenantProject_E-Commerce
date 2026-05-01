using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class OrderItem : AuditableEntity, ISoftDelete
{
    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }

    public string ProductNameSnapshot { get; set; } = default!;
    public string? ProductImageUrlSnapshot { get; set; }
    public string? VariantInfoSnapshot { get; set; }

    public string SKU { get; set; } = default!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Order Order { get; set; } = default!;
    public Product Product { get; set; } = default!;
    public ProductVariant? ProductVariant { get; set; }
}