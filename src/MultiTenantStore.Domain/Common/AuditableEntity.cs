using FiftyOne.Foundation.Mobile.Detection.Entities;

namespace MultiTenantStore.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}