namespace MultiTenantStore.Application.Auth.DTOs;

public sealed class RegisterMerchantDto
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string ConfirmPassword { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    public string StoreName { get; set; } = default!;

    public string StoreSlug { get; set; } = default!;

    public string PlanCode { get; set; } = default!;

    public string BillingCycle { get; set; } = default!;
}