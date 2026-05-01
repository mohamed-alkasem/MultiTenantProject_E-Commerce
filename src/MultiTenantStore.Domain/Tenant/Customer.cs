using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class Customer : AuditableEntity, ISoftDelete
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}