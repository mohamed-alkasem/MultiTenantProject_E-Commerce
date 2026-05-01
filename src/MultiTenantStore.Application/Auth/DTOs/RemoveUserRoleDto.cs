namespace MultiTenantStore.Application.Auth.DTOs;

public sealed class RemoveUserRoleDto
{
    public Guid UserId { get; set; }

    public string RoleName { get; set; } = default!;
}