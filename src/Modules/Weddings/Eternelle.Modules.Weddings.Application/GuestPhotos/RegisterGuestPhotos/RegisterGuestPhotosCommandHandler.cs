using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Application.Abstractions.Storage;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.RegisterGuestPhotos;

internal sealed class RegisterGuestPhotosCommandHandler(
    IWeddingRepository weddingRepository,
    IGuestPhotoRepository guestPhotoRepository,
    IUploadSlotStore uploadSlotStore,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterGuestPhotosCommand, IReadOnlyList<Guid>>
{
    public async Task<Result<IReadOnlyList<Guid>>> Handle(
        RegisterGuestPhotosCommand command,
        CancellationToken cancellationToken)
    {
        Wedding? wedding = await weddingRepository.GetByUploadTokenAsync(
            command.UploadToken,
            cancellationToken);

        if (wedding is null)
        {
            return Result.Failure<IReadOnlyList<Guid>>(
                WeddingErrors.NotFound(new WeddingId(Guid.Empty)));
        }

        // Fast-path limit check — derived from the plan tier on the wedding entity,
        // no additional DB or service call needed.
        int? planLimit = WeddingPlanLimits.PhotoLimit(wedding.Plan);

        if (planLimit is not null)
        {
            // Count only active (non-OverLimit) photos so that previously demoted photos do not
            // incorrectly block new uploads when active count is still within the plan cap.
            int current = await guestPhotoRepository.CountActiveByWeddingIdAsync(
                wedding.Id, cancellationToken);

            if (current + command.Photos.Count > planLimit.Value)
            {
                return Result.Failure<IReadOnlyList<Guid>>(GuestPhotoErrors.PlanLimitReached);
            }
        }

        // Atomically redeem all slot IDs in a single operation. If any slot is invalid or
        // already redeemed, the entire batch is rejected and no slots are consumed —
        // preventing orphaned CDN uploads that can never be registered.
        var slotIds = command.Photos.Select(r => r.SlotId).ToList();

        // Reject duplicates before touching Redis — a duplicate SlotId would cause one CDN
        // upload to be registered as two separate GuestPhoto records.
        if (new HashSet<Guid>(slotIds).Count != slotIds.Count)
        {
            return Result.Failure<IReadOnlyList<Guid>>(GuestPhotoErrors.InvalidUploadSlot);
        }

        IReadOnlyDictionary<Guid, string>? cdnUrls =
            await uploadSlotStore.RedeemManyAsync(slotIds, cancellationToken);

        if (cdnUrls is null)
        {
            return Result.Failure<IReadOnlyList<Guid>>(GuestPhotoErrors.InvalidUploadSlot);
        }

        // SnapShare must be configured before photos can be registered via the upload token.
        // In practice GetByUploadTokenAsync only returns weddings with an active SnapShare,
        // but we check explicitly to preserve invariants.
        if (wedding.SnapShare is null)
        {
            return Result.Failure<IReadOnlyList<Guid>>(WeddingErrors.SnapShareNotConfigured);
        }

        GuestPhotoStatus initialStatus = wedding.SnapShare.ModerationMode == SnapShareModerationMode.Auto
            ? GuestPhotoStatus.Approved
            : GuestPhotoStatus.Pending;

        DateTime now = dateTimeProvider.UtcNow;

        List<GuestPhoto> photos = [];
        foreach (PhotoRegistration r in command.Photos)
        {
            Result<ImageUrl> srcUrlResult = ImageUrl.Create(cdnUrls[r.SlotId]);
            if (srcUrlResult.IsFailure)
            {
                return Result.Failure<IReadOnlyList<Guid>>(srcUrlResult.Error);
            }

            PersonName? uploaderName = null;
            if (!string.IsNullOrWhiteSpace(r.UploaderName))
            {
                Result<PersonName> uploaderNameResult = PersonName.Create(r.UploaderName);
                if (uploaderNameResult.IsFailure)
                {
                    return Result.Failure<IReadOnlyList<Guid>>(uploaderNameResult.Error);
                }
                uploaderName = uploaderNameResult.Value;
            }

            photos.Add(GuestPhoto.Create(
                wedding.Id,
                srcUrlResult.Value,
                uploaderName,
                thumbnailUrl: null,   // thumbnails are CDN-derived via URL params, not a separate upload
                r.WidthPx,
                r.HeightPx,
                initialStatus,
                now));
        }

        if (planLimit is not null)
        {
            await guestPhotoRepository.InsertManyAndEnforceAsync(
                photos, wedding.Id, planLimit.Value, cancellationToken);
        }
        else
        {
            guestPhotoRepository.InsertMany(photos);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success<IReadOnlyList<Guid>>(photos.Select(p => p.Id.Value).ToList());
    }
}
