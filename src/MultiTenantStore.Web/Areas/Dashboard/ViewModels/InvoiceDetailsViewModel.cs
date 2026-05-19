using MultiTenantStore.Application.Invoices.DTOs;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class InvoiceDetailsViewModel
{
    public InvoiceDto Invoice { get; set; } = default!;
}
