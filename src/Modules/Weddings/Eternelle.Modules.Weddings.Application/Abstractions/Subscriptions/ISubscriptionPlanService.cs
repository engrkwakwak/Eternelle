namespace Eternelle.Modules.Weddings.Application.Abstractions.Subscriptions;

/// <summary>
/// Returns plan-tier limits for a given tenant.
/// Implementations read from a configuration store or a cross-module service —
/// never from the Weddings module's own tables.
/// </summary>
public interface ISubscriptionPlanService
{
    /// <summary>
    /// Returns the maximum number of guest photos allowed for the tenant's current plan,
    /// or <c>null</c> if the plan has no photo limit (unlimited).
    /// </summary>
    Task<int?> GetPhotoLimitAsync(Guid tenantId, CancellationToken cancellationToken);
}
