namespace MultiTenantStore.Application.Storefront.DTOs;

public sealed class PublicProductListDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string? NameAr { get; set; }

    public string Slug { get; set; } = default!;

    public string? ShortDescription { get; set; }

    public string? ShortDescriptionAr { get; set; }

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public string? PrimaryImageUrl { get; set; }

    public bool IsFeatured { get; set; }
}