namespace MultiTenantStore.Application.Invoices.DTOs;

public sealed class InvoiceDto
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string InvoiceNumber { get; set; } = default!;

    public string Status { get; set; } = default!;

    public DateTime IssueDate { get; set; }

    public DateTime? DueDate { get; set; }

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = default!;

    public string CustomerNameSnapshot { get; set; } = default!;

    public string? CustomerEmailSnapshot { get; set; }

    public string BillingAddressSnapshot { get; set; } = default!;

    public string StoreNameSnapshot { get; set; } = default!;

    public string? StoreTaxNumberSnapshot { get; set; }

    public string? StoreAddressSnapshot { get; set; }

    public string? PdfUrl { get; set; }

    public List<InvoiceItemDto> Items { get; set; } = new();
}