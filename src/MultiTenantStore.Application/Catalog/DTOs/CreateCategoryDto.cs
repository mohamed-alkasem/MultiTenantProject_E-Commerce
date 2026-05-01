namespace MultiTenantStore.Application.Catalog.DTOs;

public sealed class CreateCategoryDto
{
    public Guid? ParentCategoryId { get; set; }

    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }
}