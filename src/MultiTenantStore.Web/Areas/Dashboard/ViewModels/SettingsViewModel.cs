using System.ComponentModel.DataAnnotations;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class SettingsViewModel
{
    [Required(ErrorMessage = "العملة مطلوبة")]
    public string Currency { get; set; } = "SAR";

    [Required(ErrorMessage = "المنطقة الزمنية مطلوبة")]
    public string Timezone { get; set; } = "Asia/Riyadh";

    [Required(ErrorMessage = "اللغة الافتراضية مطلوبة")]
    public string DefaultLanguage { get; set; } = "ar";

    public bool IsCheckoutEnabled { get; set; }

    public bool TaxEnabled { get; set; }

    [Required(ErrorMessage = "بادئة الطلب مطلوبة")]
    public string OrderPrefix { get; set; } = "ORD-";
}
