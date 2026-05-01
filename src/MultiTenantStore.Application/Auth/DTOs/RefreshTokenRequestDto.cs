namespace MultiTenantStore.Application.Auth.DTOs;

public sealed class RefreshTokenRequestDto
{
    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;
}