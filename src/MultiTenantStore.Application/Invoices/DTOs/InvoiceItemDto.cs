namespace MultiTenantStore.Application.Invoices.DTOs;

public sealed class InvoiceItemDto
{
    public Guid Id { get; set; }

    public Guid InvoiceId { get; set; }

    public Guid? OrderItemId { get; set; }

    public string ProductNameSnapshot { get; set; } = default!;

    public string SKU { get; set; } = default!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TaxRate { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal LineTotal { get; set; }
}