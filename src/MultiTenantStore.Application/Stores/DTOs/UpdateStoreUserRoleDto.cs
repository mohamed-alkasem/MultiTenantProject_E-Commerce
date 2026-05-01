namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class UpdateStoreUserRoleDto
{
    public Guid StoreUserId { get; set; }

    public string Role { get; set; } = default!;

    public bool IsActive { get; set; }
}