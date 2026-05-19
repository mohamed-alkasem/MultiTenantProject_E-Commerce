namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class UpdateProductDto
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }

    public string Slug { get; set; } = default!;

    public string? ShortDescription { get; set; }
    public string? ShortDescriptionAr { get; set; }

    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }

    public string SKU { get; set; } = default!;

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public int StockQuantity { get; set; }

    public bool TrackInventory { get; set; }

    public int? LowStockThreshold { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }
}