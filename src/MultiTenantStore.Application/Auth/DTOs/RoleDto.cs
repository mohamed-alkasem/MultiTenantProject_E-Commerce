namespace MultiTenantStore.Application.Auth.DTOs;

public sealed class RoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
}