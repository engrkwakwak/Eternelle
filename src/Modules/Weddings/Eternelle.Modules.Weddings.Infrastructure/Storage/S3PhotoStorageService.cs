using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Eternelle.Modules.Weddings.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Eternelle.Modules.Weddings.Infrastructure.Storage;

/// <summary>
/// S3-compatible implementation of <see cref="IPhotoStorageService"/>.
/// Works with MinIO locally and Cloudflare R2 / AWS S3 in production —
/// all three expose the same S3 API surface.
/// </summary>
internal sealed class S3PhotoStorageService : IPhotoStorageService, IDisposable
{
    // Presigned PUT URLs expire after 15 minutes — matches the slot store TTL in RedisUploadSlotStore.
    private static readonly TimeSpan PresignExpiry = TimeSpan.FromMinutes(15);

    private readonly AmazonS3Client _s3;
    private readonly PhotoStorageOptions _options;
    private bool _bucketEnsured;

    public S3PhotoStorageService(IOptions<PhotoStorageOptions> options)
    {
        _options = options.Value;

        _s3 = new AmazonS3Client(
            _options.AccessKey,
            _options.SecretKey,
            new AmazonS3Config
            {
                // ForcePathStyle is required for MinIO and Cloudflare R2.
                // AWS S3 uses virtual-hosted style by default but also accepts path style.
                ServiceURL = _options.ServiceUrl,
                ForcePathStyle = true
            });
    }

    public async Task<IReadOnlyList<PresignedUploadSlot>> GeneratePresignedSlotsAsync(
        int count,
        CancellationToken cancellationToken)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        var slots = new List<PresignedUploadSlot>(count);

        for (int i = 0; i < count; i++)
        {
            var slotId = Guid.NewGuid();
            string objectKey = $"photos/{slotId}";

            string presignedUrl = await _s3.GetPreSignedURLAsync(new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.Add(PresignExpiry),
                ContentType = "image/*"
            });

            // The SDK generates the presigned URL using ServiceUrl (internal Docker hostname).
            // Replace it with PublicUrl so the browser can reach MinIO directly.
            // In production ServiceUrl == PublicUrl so this is a no-op.
            string publicPresignedUrl = presignedUrl.Replace(_options.ServiceUrl, _options.PublicUrl);
            string cdnUrl = $"{_options.PublicUrl}/{_options.BucketName}/{objectKey}";

            slots.Add(new PresignedUploadSlot(slotId, publicPresignedUrl, cdnUrl));
        }

        return slots;
    }

    /// <summary>
    /// Creates the bucket if it does not already exist.
    /// Idempotent — safe to call on every request, result is cached after the first success.
    /// </summary>
    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        if (_bucketEnsured)
        {
            return;
        }

        bool exists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3, _options.BucketName);

        if (!exists)
        {
            await _s3.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _options.BucketName,
                UseClientRegion = true
            }, cancellationToken);
        }

        _bucketEnsured = true;
    }

    public void Dispose() => _s3.Dispose();
}
