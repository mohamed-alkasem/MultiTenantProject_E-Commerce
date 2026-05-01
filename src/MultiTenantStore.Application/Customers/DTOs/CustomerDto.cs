namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class CustomerDto
{
    public Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public bool EmailConfirmed { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public List<CustomerAddressDto> Addresses { get; init; } = new();
}
