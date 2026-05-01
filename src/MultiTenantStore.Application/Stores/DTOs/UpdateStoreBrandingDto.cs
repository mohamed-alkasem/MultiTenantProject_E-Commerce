namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class UpdateStoreBrandingDto
{
    public string? LogoUrl { get; set; }

    public string? PrimaryColor { get; set; }

    public string? SecondaryColor { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? Address { get; set; }
}