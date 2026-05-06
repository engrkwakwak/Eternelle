namespace Eternelle.Modules.Weddings.Application.Abstractions.Subscriptions;

/// <summary>
/// Returns plan-tier limits for a given tenant.
/// Implementations read from a configuration store or a cross-module service —
/// never from the Weddings module's own tables.
/// </summary>
public interface ISubscriptionPlanService
{
    /// <summary>
    /// Returns the maximum number of guest photos allowed for the tenant's current plan.
    /// </summary>
    int GetPhotoLimit(Guid tenantId);
}
