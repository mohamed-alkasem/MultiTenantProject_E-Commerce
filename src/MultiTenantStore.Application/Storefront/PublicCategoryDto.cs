namespace MultiTenantStore.Application.Storefront.DTOs;

public sealed class PublicCategoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string? NameAr { get; set; }

    public string Slug { get; set; } = default!;
}