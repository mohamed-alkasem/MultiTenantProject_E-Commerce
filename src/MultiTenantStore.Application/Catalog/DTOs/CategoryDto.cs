namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class CategoryDto
{
    public Guid Id { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }

    public string Slug { get; set; } = default!;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }
}