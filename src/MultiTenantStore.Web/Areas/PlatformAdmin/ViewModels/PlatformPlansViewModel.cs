using System.ComponentModel.DataAnnotations;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

public sealed class PlatformPlansListViewModel
{
    public List<PlatformPlanRowViewModel> Plans { get; set; } = new();
}

public sealed class PlatformPlanRowViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }
    public int MaxProducts { get; set; }
    public int MaxStaffUsers { get; set; }
    public bool IsActive { get; set; }
    public int SubscriberCount { get; set; }
}

public sealed class PlatformPlanFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    public string Code { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    public string? NameAr { get; set; }

    [Range(0, 99999)]
    public decimal PriceMonthly { get; set; }

    [Range(0, 99999)]
    public decimal PriceYearly { get; set; }

    [Range(0, 100000)]
    public int MaxProducts { get; set; } = 50;

    [Range(0, 1000)]
    public int MaxStaffUsers { get; set; } = 3;

    public bool IsActive { get; set; } = true;
}
