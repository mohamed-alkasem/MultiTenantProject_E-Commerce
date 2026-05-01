using System.Security.Claims;
using MultiTenantStore.Application.Customers.Services;

namespace MultiTenantStore.Web.Services;

public sealed class CurrentCustomerService : ICurrentCustomerService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentCustomerService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? CustomerId
    {
        get
        {
            var value = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue("customer_id");

            return Guid.TryParse(value, out var customerId)
                ? customerId
                : null;
        }
    }

    public bool IsAuthenticated =>
        CustomerId is not null;
}