using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Application.Abstractions.Storage;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
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
            int current = await guestPhotoRepository.CountByWeddingIdAsync(
                wedding.Id, cancellationToken);

            if (current + command.Photos.Count > planLimit.Value)
            {
                return Result.Failure<IReadOnlyList<Guid>>(GuestPhotoErrors.PlanLimitReached);
            }
        }

        // Redeem all slot IDs — fail the entire batch if any slot is invalid or expired.
        // This prevents partial registrations where some photos are stored and others are not.
        var cdnUrls = new Dictionary<Guid, string>(command.Photos.Count);

        foreach (PhotoRegistration registration in command.Photos)
        {
            string? cdnUrl = await uploadSlotStore.RedeemAsync(registration.SlotId, cancellationToken);

            if (cdnUrl is null)
            {
                return Result.Failure<IReadOnlyList<Guid>>(GuestPhotoErrors.InvalidUploadSlot);
            }

            cdnUrls[registration.SlotId] = cdnUrl;
        }

        GuestPhotoStatus initialStatus = wedding.SnapShare!.ModerationMode == SnapShareModerationMode.Auto
            ? GuestPhotoStatus.Approved
            : GuestPhotoStatus.Pending;

        DateTime now = dateTimeProvider.UtcNow;

        List<GuestPhoto> photos = [.. command.Photos
            .Select(r => GuestPhoto.Create(
                wedding.Id,
                cdnUrls[r.SlotId],
                r.UploaderName,
                thumbnailUrl: null,   // thumbnails are CDN-derived via URL params, not a separate upload
                r.WidthPx,
                r.HeightPx,
                initialStatus,
                now))];

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
