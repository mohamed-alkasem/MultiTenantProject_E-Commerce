using Microsoft.AspNetCore.Identity;
using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Platform;

public class ApplicationUser : IdentityUser<Guid>, ISoftDelete
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Store> OwnedStores { get; set; } = new List<Store>();
    public ICollection<StoreUser> StoreUsers { get; set; } = new List<StoreUser>();
}