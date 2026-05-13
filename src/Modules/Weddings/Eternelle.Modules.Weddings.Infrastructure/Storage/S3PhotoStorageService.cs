using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Eternelle.Common.Application.Clock;
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

    // Used for bucket management — talks to the internal Docker/service endpoint.
    private readonly AmazonS3Client _s3;
    // Used for presigning — talks to the public endpoint so the signed host matches
    // what the browser will send. In production ServiceUrl == PublicUrl so both
    // clients are equivalent and no URL-rewriting is needed.
    private readonly AmazonS3Client _s3Public;
    private readonly PhotoStorageOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;

    // Guards EnsureBucketExistsAsync against concurrent initialization.
    // S3PhotoStorageService is registered as a Singleton, so multiple requests
    // can race on first startup. SemaphoreSlim + double-check prevents duplicate
    // PutBucketAsync calls.
    private readonly SemaphoreSlim _bucketLock = new(1, 1);
    private volatile bool _bucketEnsured;

    public S3PhotoStorageService(IOptions<PhotoStorageOptions> options, IDateTimeProvider dateTimeProvider)
    {
        _options = options.Value;
        _dateTimeProvider = dateTimeProvider;

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

        _s3Public = new AmazonS3Client(
            _options.AccessKey,
            _options.SecretKey,
            new AmazonS3Config
            {
                ServiceURL = _options.PublicUrl,
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

            // ContentType is intentionally omitted — including it signs the content-type into
            // the URL, which would force the client to send the exact same value or get a
            // SignatureDoesNotMatch error. Browsers send the actual MIME type on upload.
            // _s3Public is configured with PublicUrl so the signed host already matches
            // what the browser will send — no post-sign URL rewriting needed.
            string presignedUrl = await _s3Public.GetPreSignedURLAsync(new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = _dateTimeProvider.UtcNow.Add(PresignExpiry)
            });

            string cdnUrl = $"{_options.PublicUrl}/{_options.BucketName}/{objectKey}";

            slots.Add(new PresignedUploadSlot(slotId, presignedUrl, cdnUrl));
        }

        return slots;
    }

    /// <summary>
    /// Creates the bucket if it does not already exist.
    /// Thread-safe: uses a SemaphoreSlim + double-check pattern so only one caller
    /// performs the existence check and optional PutBucketAsync call, even under
    /// concurrent requests at startup.
    /// </summary>
    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        if (_bucketEnsured)
        {
            return;
        }

        await _bucketLock.WaitAsync(cancellationToken);
        try
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
        finally
        {
            _bucketLock.Release();
        }
    }

    public void Dispose()
    {
        _s3.Dispose();
        _s3Public.Dispose();
        _bucketLock.Dispose();
    }
}
