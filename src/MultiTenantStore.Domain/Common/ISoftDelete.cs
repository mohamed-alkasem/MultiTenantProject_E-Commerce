namespace MultiTenantStore.Domain.Common;

public interface ISoftDelete
{
    DateTime? DeletedAt { get; set; }
}