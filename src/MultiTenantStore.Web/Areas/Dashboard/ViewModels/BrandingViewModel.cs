using System.ComponentModel.DataAnnotations;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class BrandingViewModel
{
    public string? LogoUrl { get; set; }

    [Display(Name = "اللون الرئيسي")]
    public string? PrimaryColor { get; set; }

    [Display(Name = "اللون الثانوي")]
    public string? SecondaryColor { get; set; }

    [EmailAddress]
    [Display(Name = "بريد التواصل")]
    public string? ContactEmail { get; set; }

    [Display(Name = "هاتف التواصل")]
    public string? ContactPhone { get; set; }

    [Display(Name = "العنوان")]
    public string? Address { get; set; }

    public IFormFile? LogoFile { get; set; }
}
