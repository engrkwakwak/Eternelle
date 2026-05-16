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

    // Lua script for atomic batch redemption.
    // Reads all keys first — if any is missing, returns false (nil to the caller) without
    // deleting anything. Only when all keys exist does it delete them and return the values.
    // Redis executes Lua scripts atomically, so no slot can be partially redeemed.
    private const string RedeemManyScript = """
        local values = {}
        for i = 1, #KEYS do
            local v = redis.call('GET', KEYS[i])
            if not v then
                return false
            end
            values[i] = v
        end
        for i = 1, #KEYS do
            redis.call('DEL', KEYS[i])
        end
        return values
        """;

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

    public async Task<IReadOnlyDictionary<Guid, string>?> RedeemManyAsync(
        IReadOnlyList<Guid> slotIds,
        CancellationToken cancellationToken)
    {
        if (slotIds.Count == 0)
        {
            return new Dictionary<Guid, string>(0);
        }

        // Defensive guard — duplicate IDs would cause the Lua script to DELETE a key on the
        // second pass that was already removed, and would silently map two photos to one upload.
        if (new HashSet<Guid>(slotIds).Count != slotIds.Count)
        {
            return null;
        }

        RedisKey[] keys = [.. slotIds.Select(id => Key(id))];

        RedisResult result = await Db.ScriptEvaluateAsync(RedeemManyScript, keys);

        // Lua returned false — at least one slot was missing; nothing was deleted.
        if (result.IsNull)
        {
            return null;
        }

        var values = (RedisResult[])result!;

        var dict = new Dictionary<Guid, string>(slotIds.Count);

        for (int i = 0; i < slotIds.Count; i++)
        {
            string? cdnUrl = (string?)values[i];
            if (cdnUrl is null)
            {
                return null;
            }

            dict[slotIds[i]] = cdnUrl;
        }

        return dict;
    }
}
