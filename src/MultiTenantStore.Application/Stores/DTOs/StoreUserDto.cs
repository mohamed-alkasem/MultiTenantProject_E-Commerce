namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class StoreUserDto
{
    public Guid Id { get; set; }

    public Guid StoreId { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    public string Role { get; set; } = default!;

    public bool IsActive { get; set; }

    public DateTime? InvitedAt { get; set; }

    public DateTime? JoinedAt { get; set; }
}