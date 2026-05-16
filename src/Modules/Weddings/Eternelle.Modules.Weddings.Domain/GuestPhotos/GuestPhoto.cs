using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

/// <summary>
/// A photo uploaded by a guest for a specific wedding (wedding.guest_photos).
///
/// GuestPhoto is its own aggregate root — it has an independent lifecycle and is
/// always referenced by WeddingId, never navigated through the Wedding aggregate.
///
/// Photos begin life as Pending and move to either Approved or Rejected by the
/// couple. The couple's moderation settings may configure uploads to skip the
/// Pending state and be created as Approved immediately (initialStatus parameter
/// on Create()).
///
/// ReviewedAt is set by Approve() and Reject(). It stays null for Pending photos.
/// </summary>
public sealed class GuestPhoto : Entity
{
    private GuestPhoto()
    {
    }

    public GuestPhotoId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use IGuestPhotoRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    /// <summary>
    /// CDN or storage URL of the full-resolution photo.
    /// </summary>
    public ImageUrl SrcUrl { get; private set; }

    /// <summary>
    /// Optional CDN URL of the compressed thumbnail. Null until the storage
    /// pipeline generates it asynchronously.
    /// </summary>
    public ImageUrl? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Original pixel width. Nullable — not always available at upload time.
    /// Stored as a layout hint for the gallery renderer.
    /// </summary>
    public int? WidthPx { get; private set; }

    /// <summary>
    /// Original pixel height. Nullable — same reasoning as WidthPx.
    /// </summary>
    public int? HeightPx { get; private set; }

    /// <summary>
    /// Optional display name provided by the uploader at submission time.
    /// Null when the guest uploads anonymously.
    /// </summary>
    public PersonName? UploaderName { get; private set; }

    public GuestPhotoStatus Status { get; private set; }

    public DateTime UploadedAt { get; private set; }

    /// <summary>
    /// Timestamp of the most recent Approve() or Reject() call.
    /// Null for photos that are still Pending.
    /// </summary>
    public DateTime? ReviewedAt { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static GuestPhoto Create(
        WeddingId weddingId,
        ImageUrl srcUrl,
        PersonName? uploaderName,
        ImageUrl? thumbnailUrl,
        int? widthPx,
        int? heightPx,
        GuestPhotoStatus initialStatus,
        DateTime utcNow)
    {
        var photo = new GuestPhoto
        {
            Id = GuestPhotoId.New(),
            WeddingId = weddingId,
            SrcUrl = srcUrl,
            UploaderName = uploaderName,
            ThumbnailUrl = thumbnailUrl,
            WidthPx = widthPx,
            HeightPx = heightPx,
            Status = initialStatus,
            UploadedAt = utcNow,
            ReviewedAt = null
        };

        photo.Raise(new GuestPhotoCreatedDomainEvent(photo.Id, weddingId));

        return photo;
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Moves the photo to Approved and records the review timestamp.
    /// Returns a failure if the photo has already been reviewed.
    /// </summary>
    public Result Approve(DateTime utcNow)
    {
        if (Status != GuestPhotoStatus.Pending)
        {
            return Result.Failure(GuestPhotoErrors.AlreadyReviewed);
        }

        Status = GuestPhotoStatus.Approved;
        ReviewedAt = utcNow;

        Raise(new GuestPhotoApprovedDomainEvent(Id, WeddingId, utcNow));

        return Result.Success();
    }

    /// <summary>
    /// Moves the photo to Rejected and records the review timestamp.
    /// Returns a failure if the photo has already been reviewed.
    /// </summary>
    public Result Reject(DateTime utcNow)
    {
        if (Status != GuestPhotoStatus.Pending)
        {
            return Result.Failure(GuestPhotoErrors.AlreadyReviewed);
        }

        Status = GuestPhotoStatus.Rejected;
        ReviewedAt = utcNow;

        Raise(new GuestPhotoRejectedDomainEvent(Id, WeddingId, utcNow));

        return Result.Success();
    }

    /// <summary>
    /// Marks this photo as <see cref="GuestPhotoStatus.OverLimit"/> directly on the in-memory entity.
    /// Unlike <see cref="Approve"/> and <see cref="Reject"/>, no transition guard is applied here
    /// because bulk enforcement is performed by <c>IGuestPhotoRepository.EnforcePhotoLimitAsync</c>
    /// via a raw SQL UPDATE — the domain method is kept as a companion for completeness and
    /// single-entity scenarios, not as the primary enforcement path.
    /// </summary>
    public void MarkOverLimit()
    {
        Status = GuestPhotoStatus.OverLimit;
    }
}
