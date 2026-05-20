using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiTenantStore.Application.Catalog.DTOs;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class ProductFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "الفئة مطلوبة")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "اسم المنتج مطلوب")]
    public string Name { get; set; } = default!;

    public string? NameAr { get; set; }

    [Required(ErrorMessage = "الرابط المختصر مطلوب")]
    public string Slug { get; set; } = default!;

    public string? ShortDescription { get; set; }
    public string? ShortDescriptionAr { get; set; }

    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }

    [Required(ErrorMessage = "الرمز التعريفي (SKU) مطلوب")]
    public string SKU { get; set; } = default!;

    [Required(ErrorMessage = "السعر مطلوب")]
    [Range(0.01, double.MaxValue, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون صحيحة")]
    public int StockQuantity { get; set; }

    public bool TrackInventory { get; set; } = true;
    public int? LowStockThreshold { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
}
