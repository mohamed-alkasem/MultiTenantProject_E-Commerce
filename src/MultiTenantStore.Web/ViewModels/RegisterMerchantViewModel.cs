using System.ComponentModel.DataAnnotations;

namespace MultiTenantStore.Web.ViewModels;

public sealed class RegisterMerchantViewModel
{
    [Required]
    public string FirstName { get; set; } = default!;

    [Required]
    public string LastName { get; set; } = default!;

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = default!;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    [Required]
    public string StoreName { get; set; } = default!;

    [Required]
    public string StoreSlug { get; set; } = default!;

    [Required]
    public string PlanCode { get; set; } = default!;

    [Required]
    public string BillingCycle { get; set; } = "Monthly";

    public List<PlanOptionViewModel> Plans { get; set; } = new();
}

public sealed class PlanOptionViewModel
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }
}
