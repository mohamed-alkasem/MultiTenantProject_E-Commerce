namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class OrderItemDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public Guid? ProductVariantId { get; init; }
    public required string ProductNameSnapshot { get; init; }
    public string? ProductImageUrlSnapshot { get; init; }
    public string? VariantInfoSnapshot { get; init; }
    public required string SKU { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
