using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class ProductVariant : AuditableEntity, ISoftDelete
{
    public Guid ProductId { get; set; }

    public string SKU { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }

    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }

    public int StockQuantity { get; set; }
    public bool TrackInventory { get; set; } = true;

    public string? AttributesJson { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? DeletedAt { get; set; }

    public Product Product { get; set; } = default!;
}