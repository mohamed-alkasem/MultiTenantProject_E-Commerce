using MultiTenantStore.Application.Invoices.DTOs;
using MultiTenantStore.Application.Invoices.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MultiTenantStore.Infrastructure.Invoices;

public sealed class InvoicePdfGenerator : IInvoicePdfGenerator
{
    // Bilingual label pairs: (English, Arabic)
    private static string L(string en, string ar) => $"{en} / {ar}";

    public byte[] Generate(InvoiceDto invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // Use a font that supports Arabic (Tahoma is available on Windows Server)
        const string font = "Tahoma";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontFamily(font).FontSize(9.5f));

                // ── HEADER ─────────────────────────────────────────────────────
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text(invoice.StoreNameSnapshot)
                                .FontSize(18).Bold();
                            left.Item().Text(L("Invoice", "فاتورة"))
                                .FontSize(13).SemiBold().FontColor(Colors.Grey.Darken2);
                        });

                        row.ConstantItem(160).Column(right =>
                        {
                            right.Item().AlignRight()
                                .Text($"{L("Invoice No", "رقم الفاتورة")}: {invoice.InvoiceNumber}")
                                .FontSize(9);
                            right.Item().AlignRight()
                                .Text($"{L("Date", "التاريخ")}: {invoice.IssueDate:yyyy-MM-dd}")
                                .FontSize(9);
                            right.Item().AlignRight()
                                .Text($"{L("Status", "الحالة")}: {invoice.Status}")
                                .FontSize(9)
                                .FontColor(invoice.Status == "Paid" ? Colors.Green.Darken2 : Colors.Orange.Darken2);
                        });
                    });

                    col.Item().PaddingTop(6)
                        .BorderTop(1).BorderColor(Colors.Grey.Lighten1);
                });

                // ── CONTENT ────────────────────────────────────────────────────
                page.Content().PaddingVertical(14).Column(col =>
                {
                    col.Spacing(14);

                    // Bill-to / Store row
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text(L("Bill To", "العميل"))
                                .Bold().FontSize(10)
                                .FontColor(Colors.Grey.Darken2);
                            left.Item().PaddingTop(3)
                                .Text(invoice.CustomerNameSnapshot);
                            if (!string.IsNullOrWhiteSpace(invoice.BillingAddressSnapshot))
                                left.Item().Text(invoice.BillingAddressSnapshot)
                                    .FontColor(Colors.Grey.Darken1);
                            if (!string.IsNullOrWhiteSpace(invoice.CustomerEmailSnapshot))
                                left.Item().Text(invoice.CustomerEmailSnapshot)
                                    .FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(20);

                        row.RelativeItem().Column(right =>
                        {
                            right.Item().Text(L("Store", "المتجر"))
                                .Bold().FontSize(10)
                                .FontColor(Colors.Grey.Darken2);
                            right.Item().PaddingTop(3)
                                .Text(invoice.StoreNameSnapshot);
                            if (!string.IsNullOrWhiteSpace(invoice.StoreAddressSnapshot))
                                right.Item().Text(invoice.StoreAddressSnapshot)
                                    .FontColor(Colors.Grey.Darken1);
                            if (!string.IsNullOrWhiteSpace(invoice.StoreTaxNumberSnapshot))
                                right.Item()
                                    .Text($"{L("Tax No", "الرقم الضريبي")}: {invoice.StoreTaxNumberSnapshot}")
                                    .FontColor(Colors.Grey.Darken1);
                        });
                    });

                    // Items table
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(5);   // Product
                            cols.RelativeColumn(2);   // SKU
                            cols.RelativeColumn(1);   // Qty
                            cols.RelativeColumn(2);   // Unit
                            cols.RelativeColumn(2);   // Total
                        });

                        table.Header(h =>
                        {
                            h.Cell().Element(HdrCell).Text(L("Product", "المنتج"));
                            h.Cell().Element(HdrCell).Text(L("SKU", "الرمز"));
                            h.Cell().Element(HdrCell).AlignCenter().Text(L("Qty", "الكمية"));
                            h.Cell().Element(HdrCell).AlignRight().Text(L("Unit Price", "السعر"));
                            h.Cell().Element(HdrCell).AlignRight().Text(L("Total", "الإجمالي"));
                        });

                        foreach (var item in invoice.Items)
                        {
                            table.Cell().Element(BodyCell).Text(item.ProductNameSnapshot);
                            table.Cell().Element(BodyCell).Text(item.SKU ?? "—");
                            table.Cell().Element(BodyCell).AlignCenter().Text(item.Quantity.ToString());
                            table.Cell().Element(BodyCell).AlignRight().Text($"{item.UnitPrice:N2}");
                            table.Cell().Element(BodyCell).AlignRight().Text($"{item.LineTotal:N2}");
                        }
                    });

                    // Totals
                    col.Item().AlignRight().Width(220).Column(totals =>
                    {
                        totals.Spacing(2);

                        void TotalRow(string label, decimal value, bool bold = false)
                        {
                            totals.Item().Row(r =>
                            {
                                var labelStyle = r.RelativeItem().Text(label).FontSize(bold ? 10.5f : 9.5f);
                                if (bold) labelStyle.Bold();
                                var valueStyle = r.ConstantItem(90).AlignRight()
                                    .Text($"{value:N2} {invoice.Currency}").FontSize(bold ? 10.5f : 9.5f);
                                if (bold) valueStyle.Bold();
                            });
                        }

                        TotalRow(L("Subtotal", "المجموع الفرعي"), invoice.Subtotal);

                        if (invoice.DiscountAmount > 0)
                            TotalRow(L("Discount", "الخصم"), -invoice.DiscountAmount);

                        if (invoice.TaxAmount > 0)
                            TotalRow(L("Tax", "الضريبة"), invoice.TaxAmount);

                        if (invoice.ShippingAmount > 0)
                            TotalRow(L("Shipping", "الشحن"), invoice.ShippingAmount);

                        totals.Item().PaddingTop(4).BorderTop(1).BorderColor(Colors.Grey.Lighten1);
                        TotalRow(L("Total", "الإجمالي"), invoice.TotalAmount, bold: true);
                    });
                });

                // ── FOOTER ─────────────────────────────────────────────────────
                page.Footer().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(s => s.FontSize(8).FontColor(Colors.Grey.Lighten1));
                    text.Span(invoice.StoreNameSnapshot);
                    text.Span("  •  ");
                    text.Span(L("Generated by MultiTenantStore", "تم الإنشاء بواسطة المنصة"));
                    text.Span("  •  ");
                    text.Span(L("Page", "صفحة") + " ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    private static IContainer HdrCell(IContainer c) =>
        c.Background(Colors.Grey.Lighten3)
         .Padding(5)
         .BorderBottom(1)
         .BorderColor(Colors.Grey.Lighten2);

    private static IContainer BodyCell(IContainer c) =>
        c.Padding(5)
         .BorderBottom(0.5f)
         .BorderColor(Colors.Grey.Lighten3);
}
