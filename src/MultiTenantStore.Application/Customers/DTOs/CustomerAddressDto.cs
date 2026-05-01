namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class CustomerAddressDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public required string Title { get; init; }
    public required string FullName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public string? District { get; init; }
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? PostalCode { get; init; }
    public bool IsDefaultShipping { get; init; }
    public bool IsDefaultBilling { get; init; }
}
