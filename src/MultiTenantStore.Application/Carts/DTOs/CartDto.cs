namespace MultiTenantStore.Application.Carts.DTOs;

public sealed class CartDto
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public string Status { get; set; } = default!;
    public decimal Subtotal { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
}
