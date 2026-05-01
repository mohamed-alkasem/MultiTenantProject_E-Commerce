namespace MultiTenantStore.Application.Invoices.DTOs;

public sealed class InvoiceListDto
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string InvoiceNumber { get; set; } = default!;

    public string Status { get; set; } = default!;

    public DateTime IssueDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = default!;

    public string CustomerNameSnapshot { get; set; } = default!;

    public string? PdfUrl { get; set; }
}