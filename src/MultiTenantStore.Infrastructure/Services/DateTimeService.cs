using MultiTenantStore.Application.Common.Interfaces;

namespace MultiTenantStore.Infrastructure.Services;

public sealed class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}