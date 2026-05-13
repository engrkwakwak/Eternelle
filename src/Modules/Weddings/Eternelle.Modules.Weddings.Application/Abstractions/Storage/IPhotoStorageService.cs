namespace Eternelle.Modules.Weddings.Application.Abstractions.Storage;

/// <summary>
/// Generates presigned upload slots against the configured CDN provider.
/// The implementation is CDN-specific (Cloudflare Images, S3, etc.) and lives in Infrastructure.
/// </summary>
public interface IPhotoStorageService
{
    /// <summary>
    /// Generates <paramref name="count"/> presigned upload slots.
    /// Each slot contains the presigned upload URL and the final CDN URL
    /// (the CDN URL is deterministic at presign time for all supported providers).
    /// </summary>
    Task<IReadOnlyList<PresignedUploadSlot>> GeneratePresignedSlotsAsync(
        int count,
        CancellationToken cancellationToken);
}
