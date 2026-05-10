using Eternelle.Modules.Weddings.Application.Abstractions.Subscriptions;

namespace Eternelle.Modules.Weddings.Infrastructure.Subscriptions;

/// <summary>
/// Stub implementation — returns null (unlimited) for all tenants until the
/// Subscriptions module is wired up and a real cross-module service is available.
/// </summary>
internal sealed class StubSubscriptionPlanService : ISubscriptionPlanService
{
    public Task<int?> GetPhotoLimitAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return Task.FromResult<int?>(null);
    }
}
