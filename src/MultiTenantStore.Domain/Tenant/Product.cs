using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class Product : AuditableEntity, ISoftDelete
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;

    public string? ShortDescription { get; set; }
    public string? Description { get; set; }

    public string SKU { get; set; } = default!;

    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }

    public int StockQuantity { get; set; }
    public bool TrackInventory { get; set; } = true;
    public int? LowStockThreshold { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
    public DateTime? PublishedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Category Category { get; set; } = default!;

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}