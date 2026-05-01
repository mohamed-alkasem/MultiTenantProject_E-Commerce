namespace MultiTenantStore.Application.Customers.Services;

public interface ICurrentCustomerService
{
    Guid? CustomerId { get; }

    bool IsAuthenticated { get; }
}