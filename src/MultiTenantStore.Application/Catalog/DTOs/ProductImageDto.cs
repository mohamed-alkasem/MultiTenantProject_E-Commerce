namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class ProductImageDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = default!;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}