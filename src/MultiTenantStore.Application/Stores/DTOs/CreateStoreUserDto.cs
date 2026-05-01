namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class CreateStoreUserDto
{
    public Guid StoreId { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = default!;
}