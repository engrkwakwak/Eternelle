using Eternelle.Modules.Weddings.Application.Abstractions.Storage;
using Microsoft.Extensions.Caching.Distributed;

namespace Eternelle.Modules.Weddings.Infrastructure.Storage;

/// <summary>
/// Redis-backed implementation of <see cref="IUploadSlotStore"/>.
/// Slots expire after <see cref="SlotTtl"/> and are deleted on first redemption.
/// The get-then-delete is not atomic; the tiny race window is acceptable for this use case.
/// </summary>
internal sealed class RedisUploadSlotStore(IDistributedCache cache) : IUploadSlotStore
{
    private static readonly TimeSpan SlotTtl = TimeSpan.FromMinutes(15);

    private static string Key(Guid slotId) => $"upload-slot:{slotId}";

    public Task StoreAsync(Guid slotId, string cdnUrl, CancellationToken cancellationToken)
    {
        return cache.SetStringAsync(
            Key(slotId),
            cdnUrl,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = SlotTtl
            },
            cancellationToken);
    }

    public async Task<string?> RedeemAsync(Guid slotId, CancellationToken cancellationToken)
    {
        string key = Key(slotId);
        string? cdnUrl = await cache.GetStringAsync(key, cancellationToken);

        if (cdnUrl is not null)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }

        return cdnUrl;
    }
}
