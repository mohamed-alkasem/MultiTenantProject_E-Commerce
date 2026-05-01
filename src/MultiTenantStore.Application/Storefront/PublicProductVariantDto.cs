namespace MultiTenantStore.Application.Storefront.DTOs;

public sealed class PublicProductVariantDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string SKU { get; set; } = default!;

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public int StockQuantity { get; set; }

    public string? AttributesJson { get; set; }
}