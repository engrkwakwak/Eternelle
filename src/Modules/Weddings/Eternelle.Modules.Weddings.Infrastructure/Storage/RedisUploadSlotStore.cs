using Eternelle.Modules.Weddings.Application.Abstractions.Storage;
using StackExchange.Redis;

namespace Eternelle.Modules.Weddings.Infrastructure.Storage;

/// <summary>
/// Redis-backed implementation of <see cref="IUploadSlotStore"/>.
/// Slots expire after <see cref="SlotTtl"/> and are atomically fetched-and-deleted
/// on redemption using the Redis GETDEL command, preventing double-redemption races.
/// </summary>
internal sealed class RedisUploadSlotStore(IConnectionMultiplexer redis) : IUploadSlotStore
{
    private static readonly TimeSpan SlotTtl = TimeSpan.FromMinutes(15);

    private IDatabase Db => redis.GetDatabase();

    private static RedisKey Key(Guid slotId) => $"upload-slot:{slotId}";

    public Task StoreAsync(Guid slotId, string cdnUrl, CancellationToken cancellationToken)
    {
        return Db.StringSetAsync(Key(slotId), cdnUrl, SlotTtl);
    }

    public async Task<string?> RedeemAsync(Guid slotId, CancellationToken cancellationToken)
    {
        // GETDEL atomically returns the value and removes the key in a single round-trip.
        // This eliminates the race window that existed between the previous GetStringAsync
        // and RemoveAsync calls, preventing a slot from being redeemed more than once.
        RedisValue value = await Db.StringGetDeleteAsync(Key(slotId));

        return value.HasValue ? (string?)value : null;
    }
}
