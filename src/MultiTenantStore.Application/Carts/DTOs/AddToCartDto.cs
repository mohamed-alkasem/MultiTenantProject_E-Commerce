namespace MultiTenantStore.Application.Carts.DTOs;

public sealed class AddToCartDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public int Quantity { get; set; }
}
