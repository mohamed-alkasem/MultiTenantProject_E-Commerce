namespace MultiTenantStore.Application.Common.Interfaces;

public interface IDateTimeService
{
    DateTime UtcNow { get; }
}