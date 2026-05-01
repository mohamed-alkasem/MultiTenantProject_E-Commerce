using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class CartItem : AuditableEntity, ISoftDelete
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Cart Cart { get; set; } = default!;
    public Product Product { get; set; } = default!;
    public ProductVariant? ProductVariant { get; set; }
}