namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class CustomerLoginDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
