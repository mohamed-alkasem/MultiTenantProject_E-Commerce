namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class ProductVariantDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string SKU { get; set; } = default!;

    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public int StockQuantity { get; set; }

    public bool TrackInventory { get; set; }

    public string? AttributesJson { get; set; }

    public bool IsActive { get; set; }
}