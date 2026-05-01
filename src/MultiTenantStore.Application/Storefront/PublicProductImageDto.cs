namespace MultiTenantStore.Application.Storefront.DTOs;

public sealed class PublicProductImageDto
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = default!;

    public string? AltText { get; set; }

    public int SortOrder { get; set; }

    public bool IsPrimary { get; set; }
}