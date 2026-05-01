using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class CustomerAddress : AuditableEntity, ISoftDelete
{
    public Guid CustomerId { get; set; }

    public string Title { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;

    public string Country { get; set; } = default!;
    public string City { get; set; } = default!;
    public string? District { get; set; }

    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }

    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Customer Customer { get; set; } = default!;
}