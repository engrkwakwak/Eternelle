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
}
