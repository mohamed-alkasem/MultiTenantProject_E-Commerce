namespace MultiTenantStore.Application.Auth.DTOs;

public sealed class UpdatePlatformUserDto
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }
}