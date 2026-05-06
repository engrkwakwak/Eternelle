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

        // 2. Plan limit check.
        int count = await guestPhotoRepository.CountByWeddingIdAsync(
            wedding.Id,
            cancellationToken);

        int planLimit = subscriptionPlanService.GetPhotoLimit(wedding.TenantId);

        if (count >= planLimit)
        {
            return Result.Failure<Guid>(GuestPhotoErrors.PlanLimitReached);
        }

        // 3. Determine initial status based on the couple's moderation preference.
        GuestPhotoStatus initialStatus = wedding.SnapShare!.ModerationMode == SnapShareModerationMode.Auto
            ? GuestPhotoStatus.Approved
            : GuestPhotoStatus.Pending;

        // 4. Create and persist.
        var photo = GuestPhoto.Create(
            wedding.Id,
            command.SrcUrl,
            command.UploaderName,
            command.ThumbnailUrl,
            command.WidthPx,
            command.HeightPx,
            initialStatus,
            dateTimeProvider.UtcNow);

        guestPhotoRepository.Insert(photo);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(photo.Id.Value);
    }
}
