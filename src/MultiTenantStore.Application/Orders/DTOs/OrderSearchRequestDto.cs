namespace MultiTenantStore.Application.Orders.DTOs;

public sealed class OrderSearchRequestDto
{
    public string? Search { get; init; }
    public string? Status { get; init; }
    public string? PaymentStatus { get; init; }
    public string? ShippingStatus { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
