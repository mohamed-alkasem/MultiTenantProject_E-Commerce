using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class Category : AuditableEntity, ISoftDelete
{
    public Guid? ParentCategoryId { get; set; }

    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Category? ParentCategory { get; set; }
    public ICollection<Category> Children { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}