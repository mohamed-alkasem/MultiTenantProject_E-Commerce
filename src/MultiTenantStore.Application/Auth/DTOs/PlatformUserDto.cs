namespace MultiTenantStore.Application.Auth.DTOs;

public sealed class PlatformUserDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public bool EmailConfirmed { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public List<string> Roles { get; set; } = new();
}