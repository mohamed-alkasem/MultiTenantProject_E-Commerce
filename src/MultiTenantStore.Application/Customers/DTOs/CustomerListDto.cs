namespace MultiTenantStore.Application.Customers.DTOs;

public sealed class CustomerListDto
{
    public Guid Id { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public int OrdersCount { get; init; }
    public decimal TotalSpent { get; init; }
}
