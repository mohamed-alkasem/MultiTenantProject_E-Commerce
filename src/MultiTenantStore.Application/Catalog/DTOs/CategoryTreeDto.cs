namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class CategoryTreeDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }

    public string Slug { get; set; } = default!;

    public List<CategoryTreeDto> Children { get; set; } = new();
}