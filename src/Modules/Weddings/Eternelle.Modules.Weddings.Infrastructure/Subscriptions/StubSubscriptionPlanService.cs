using Eternelle.Modules.Weddings.Application.Abstractions.Subscriptions;
using Microsoft.Extensions.Options;

namespace Eternelle.Modules.Weddings.Infrastructure.Subscriptions;

/// <summary>
/// Stub implementation — reads limits from <see cref="SubscriptionOptions"/> until the
/// Subscriptions module is wired up and a real cross-module service is available.
/// </summary>
internal sealed class StubSubscriptionPlanService(IOptions<SubscriptionOptions> options) : ISubscriptionPlanService
{
    public Task<int?> GetPhotoLimitAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        return Task.FromResult(options.Value.PhotoUploadLimit);
    }
}
