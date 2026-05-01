using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Platform;

public class StoreUser : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }
    public Guid UserId { get; set; }

    public StoreUserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime? InvitedAt { get; set; }
    public DateTime? JoinedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Store Store { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
}