namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class CreateStoreDatabaseDto
{
    public Guid StoreId { get; set; }

    public string DatabaseName { get; set; } = default!;

    public string DbServer { get; set; } = default!;

    public string Provider { get; set; } = default!;
}