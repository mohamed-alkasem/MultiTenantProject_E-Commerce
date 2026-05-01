namespace MultiTenantStore.Application.Auth.DTOs;

public sealed class AuthResponseDto
{
    public Guid UserId { get; set; }

    public Guid? StoreId { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? PhoneNumber { get; set; }

    public string GlobalRole { get; set; } = default!;

    public string? StoreRole { get; set; }

    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;

    public DateTime ExpiresAt { get; set; }
}