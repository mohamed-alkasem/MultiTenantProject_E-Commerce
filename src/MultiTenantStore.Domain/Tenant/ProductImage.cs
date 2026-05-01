using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class ProductImage : AuditableEntity, ISoftDelete
{
    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = default!;
    public string? AltText { get; set; }

    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Product Product { get; set; } = default!;
}