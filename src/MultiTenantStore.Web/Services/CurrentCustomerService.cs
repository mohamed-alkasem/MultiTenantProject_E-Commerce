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
            // Try JWT/cookie claims first
            var claimValue = _httpContextAccessor?.HttpContext?.User.FindFirstValue("customer_id");
            if (Guid.TryParse(claimValue, out var fromClaims))
                return fromClaims;

            // Fallback: MVC storefront uses session-based auth
            var sessionValue = _httpContextAccessor?.HttpContext?.Session.GetString("CustomerId");
            return Guid.TryParse(sessionValue, out var fromSession) ? fromSession : null;
        }
    }

    public bool IsAuthenticated =>
        CustomerId is not null;
}