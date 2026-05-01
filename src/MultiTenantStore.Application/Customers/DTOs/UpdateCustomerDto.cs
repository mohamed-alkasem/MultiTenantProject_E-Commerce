namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class UpdateCustomerDto
{
    public Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
}
