namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class RegisterCustomerDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
    public string? PhoneNumber { get; init; }
}
