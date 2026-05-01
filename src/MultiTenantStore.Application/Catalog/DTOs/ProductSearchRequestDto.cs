namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class ProductSearchRequestDto
{
    public string? Search { get; set; }

    public Guid? CategoryId { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsFeatured { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}