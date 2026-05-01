namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class CreateCustomerDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Password { get; init; }
}
