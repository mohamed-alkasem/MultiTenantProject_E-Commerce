using System.ComponentModel.DataAnnotations;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

public sealed class AdminLoginViewModel
{
    [Required(ErrorMessage = "البريد الإلكتروني مطلوب / Email is required")]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "كلمة المرور مطلوبة / Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    public bool RememberMe { get; set; }
}
