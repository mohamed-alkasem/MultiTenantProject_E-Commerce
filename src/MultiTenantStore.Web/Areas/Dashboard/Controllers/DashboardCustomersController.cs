using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardCustomersController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "العملاء";
        return View();
    }
}
