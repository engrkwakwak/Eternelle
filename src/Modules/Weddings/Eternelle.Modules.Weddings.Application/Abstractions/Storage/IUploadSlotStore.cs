namespace Eternelle.Modules.Weddings.Application.Abstractions.Storage;

/// <summary>
/// Short-lived store that maps a server-issued slot ID to its CDN URL.
/// Slots expire after a fixed TTL and are deleted on first redemption
/// to prevent reuse.
/// </summary>
public interface IUploadSlotStore
{
    /// <summary>Persists a slot with a fixed TTL.</summary>
    Task StoreAsync(Guid slotId, string cdnUrl, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the CDN URL for <paramref name="slotId"/> and atomically removes it.
    /// Returns <see langword="null"/> if the slot has expired or was already redeemed.
    /// </summary>
    Task<string?> RedeemAsync(Guid slotId, CancellationToken cancellationToken);

    /// <summary>
    /// Atomically validates and redeems all <paramref name="slotIds"/> in a single operation.
    /// Returns a dictionary mapping each slot ID to its CDN URL if every slot is valid,
    /// or <see langword="null"/> if any slot is missing or already redeemed — in which case
    /// no slots are consumed.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, string>?> RedeemManyAsync(
        IReadOnlyList<Guid> slotIds,
        CancellationToken cancellationToken);
}
