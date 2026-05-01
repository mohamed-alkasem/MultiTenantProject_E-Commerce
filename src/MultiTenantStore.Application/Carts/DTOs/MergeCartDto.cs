namespace MultiTenantStore.Application.Carts.DTOs;

public sealed class MergeCartDto
{
    public required string SessionId { get; init; }
    public Guid CustomerId { get; init; }
}
