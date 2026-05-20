using System.ComponentModel.DataAnnotations;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class DashboardProfileViewModel
{
    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";
}

public sealed class DashboardChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = "";

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = "";
}
