using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Tenant;

public class Cart : AuditableEntity, ISoftDelete
{
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }

    public CartStatus Status { get; set; } = CartStatus.Active;

    public DateTime? DeletedAt { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}