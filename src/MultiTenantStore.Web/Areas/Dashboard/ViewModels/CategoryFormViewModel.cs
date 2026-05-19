using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class CategoryFormViewModel
{
    public Guid? Id { get; set; }

    public Guid? ParentCategoryId { get; set; }

    [Required(ErrorMessage = "اسم الفئة مطلوب")]
    public string Name { get; set; } = default!;

    public string? NameAr { get; set; }

    [Required(ErrorMessage = "الرابط المختصر مطلوب")]
    public string Slug { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public List<SelectListItem> ParentCategories { get; set; } = new();
}
