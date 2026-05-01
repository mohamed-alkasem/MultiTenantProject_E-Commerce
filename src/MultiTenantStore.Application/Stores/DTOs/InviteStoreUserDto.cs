namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class InviteStoreUserDto
{
    public Guid StoreId { get; set; }

    public string Email { get; set; } = default!;

    public string Role { get; set; } = default!;
}