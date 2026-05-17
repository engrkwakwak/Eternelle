using System.Collections.Concurrent;
using Eternelle.Modules.Weddings.Application.Abstractions.Storage;

namespace Eternelle.Modules.Weddings.Infrastructure.Storage;

/// <summary>
/// In-memory fallback implementation of <see cref="IUploadSlotStore"/> used when Redis is
/// unavailable (e.g. local development without a running Redis instance).
/// Slots are stored in a process-scoped <see cref="ConcurrentDictionary"/>; TTL is not enforced.
/// Not suitable for production or multi-instance deployments.
/// </summary>
internal sealed class StubUploadSlotStore : IUploadSlotStore
{
    private readonly ConcurrentDictionary<Guid, string> _slots = new();
    private readonly object _redeemLock = new();

    public Task StoreAsync(Guid slotId, string cdnUrl, CancellationToken cancellationToken)
    {
        _slots[slotId] = cdnUrl;
        return Task.CompletedTask;
    }

    public Task<string?> RedeemAsync(Guid slotId, CancellationToken cancellationToken)
    {
        string? cdnUrl = _slots.TryRemove(slotId, out string? value) ? value : null;
        return Task.FromResult(cdnUrl);
    }

    public Task<IReadOnlyDictionary<Guid, string>?> RedeemManyAsync(
        IReadOnlyList<Guid> slotIds,
        CancellationToken cancellationToken)
    {
        if (slotIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyDictionary<Guid, string>?>(new Dictionary<Guid, string>(0));
        }

        lock (_redeemLock)
        {
            // Verify all slots exist before removing any (all-or-nothing, mirrors Redis Lua script behaviour).
            foreach (Guid slotId in slotIds)
            {
                if (!_slots.ContainsKey(slotId))
                {
                    return Task.FromResult<IReadOnlyDictionary<Guid, string>?>(null);
                }
            }

            var dict = new Dictionary<Guid, string>(slotIds.Count);
            foreach (Guid slotId in slotIds)
            {
                _slots.TryRemove(slotId, out string? cdnUrl);
                dict[slotId] = cdnUrl!;
            }

            return Task.FromResult<IReadOnlyDictionary<Guid, string>?>(dict);
        }
    }
}
