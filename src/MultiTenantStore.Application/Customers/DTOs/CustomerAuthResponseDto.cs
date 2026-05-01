namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class CustomerAuthResponseDto
{
    public Guid CustomerId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public DateTime ExpiresAt { get; init; }
}
