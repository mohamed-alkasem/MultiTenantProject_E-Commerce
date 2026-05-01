namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class OrderAddressDto
{
    public required string FullName { get; init; }
    public required string Phone { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public string? District { get; init; }
    public required string AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? PostalCode { get; init; }
}
