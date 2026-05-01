namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class CreateProductImageDto
{
    public string ImageUrl { get; set; } = default!;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}