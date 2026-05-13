using Eternelle.Modules.Weddings.Application.Abstractions.Storage;

namespace Eternelle.Modules.Weddings.Infrastructure.Storage;

/// <summary>
/// Stub implementation of <see cref="IPhotoStorageService"/> used until a real CDN provider
/// (Cloudflare Images, S3, etc.) is configured.
/// Returns synthetic presigned URLs that are not backed by any real storage.
/// </summary>
internal sealed class StubPhotoStorageService : IPhotoStorageService
{
    private const string BaseUrl = "https://stub-cdn.eternelle.ph";

    public Task<IReadOnlyList<PresignedUploadSlot>> GeneratePresignedSlotsAsync(
        int count,
        CancellationToken cancellationToken)
    {
        List<PresignedUploadSlot> slots = [.. Enumerable.Range(0, count)
            .Select(_ =>
            {
                var slotId = Guid.NewGuid();
                return new PresignedUploadSlot(
                    SlotId: slotId,
                    PresignedUrl: $"{BaseUrl}/presign/{slotId}",
                    CdnUrl: $"{BaseUrl}/images/{slotId}");
            })];

        return Task.FromResult<IReadOnlyList<PresignedUploadSlot>>(slots);
    }
}
