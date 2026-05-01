namespace MultiTenantStore.Application.Carts.DTOs;

public sealed class UpdateCartItemDto
{
    public Guid CartItemId { get; set; }
    public int Quantity { get; set; }
}
