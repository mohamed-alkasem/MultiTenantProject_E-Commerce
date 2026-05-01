namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class StoreUserListDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Role { get; set; } = default!;

    public bool IsActive { get; set; }
}