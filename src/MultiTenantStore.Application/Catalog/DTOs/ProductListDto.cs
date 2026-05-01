namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class ProductListDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string SKU { get; set; } = default!;

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public bool IsFeatured { get; set; }

    public string? PrimaryImageUrl { get; set; }
}