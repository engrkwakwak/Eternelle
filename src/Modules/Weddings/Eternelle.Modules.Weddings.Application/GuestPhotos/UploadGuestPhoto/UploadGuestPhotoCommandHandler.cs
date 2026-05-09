using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Application.Abstractions.Subscriptions;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.UploadGuestPhoto;

internal sealed class UploadGuestPhotoCommandHandler(
    IWeddingRepository weddingRepository,
    IGuestPhotoRepository guestPhotoRepository,
    ISubscriptionPlanService subscriptionPlanService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<UploadGuestPhotoCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        UploadGuestPhotoCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Resolve wedding from token — null return deliberately matches NotFound
        //    so callers cannot probe token validity.
        Wedding? wedding = await weddingRepository.GetByUploadTokenAsync(
            command.UploadToken,
            cancellationToken);

        if (wedding is null)
        {
            return Result.Failure<Guid>(WeddingErrors.NotFound(new WeddingId(Guid.Empty)));
        }

        // 2. Fast-path limit check — blocks clearly-over-cap weddings without locking.
        //    A small burst of concurrent uploads may slip past; step 5 corrects them.
        int? planLimit = await subscriptionPlanService.GetPhotoLimitAsync(
            wedding.TenantId, cancellationToken);

        if (planLimit is not null)
        {
            int count = await guestPhotoRepository.CountByWeddingIdAsync(
                wedding.Id, cancellationToken);

            if (count >= planLimit.Value)
            {
                return Result.Failure<Guid>(GuestPhotoErrors.PlanLimitReached);
            }
        }

        // 3. Determine initial status based on the couple's moderation preference.
        GuestPhotoStatus initialStatus = wedding.SnapShare!.ModerationMode == SnapShareModerationMode.Auto
            ? GuestPhotoStatus.Approved
            : GuestPhotoStatus.Pending;

        // 4. Insert and enforce atomically.
        //    Capped plans use a single transaction so no committed photo can exist
        //    outside the limit window. Unlimited plans skip enforcement entirely.
        var photo = GuestPhoto.Create(
            wedding.Id,
            command.SrcUrl,
            command.UploaderName,
            command.ThumbnailUrl,
            command.WidthPx,
            command.HeightPx,
            initialStatus,
            dateTimeProvider.UtcNow);

        if (planLimit is not null)
        {
            await guestPhotoRepository.InsertAndEnforceAsync(
                photo, wedding.Id, planLimit.Value, cancellationToken);
        }
        else
        {
            guestPhotoRepository.Insert(photo);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(photo.Id.Value);
    }
}
