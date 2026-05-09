using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkRejectGuestPhotos;

internal sealed class BulkRejectGuestPhotosCommandHandler(
    IGuestPhotoRepository guestPhotoRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<BulkRejectGuestPhotosCommand>
{
    public async Task<Result> Handle(BulkRejectGuestPhotosCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<GuestPhotoId> ids = command.GuestPhotoIds
            .Select(id => new GuestPhotoId(id))
            .Distinct()
            .ToList();

        IReadOnlyList<GuestPhoto> photos = await guestPhotoRepository.GetManyAsync(ids, cancellationToken);

        if (photos.Count != ids.Count)
        {
            return Result.Failure(GuestPhotoErrors.NotFound);
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        bool hasFailures = false;

        foreach (GuestPhoto photo in photos)
        {
            Result result = photo.Reject(utcNow);

            if (!result.IsSuccess)
            {
                hasFailures = true;
            }
        }

        if (hasFailures)
        {
            return Result.Failure(GuestPhotoErrors.AlreadyReviewed);
        }

        foreach (GuestPhoto photo in photos)
        {
            guestPhotoRepository.Update(photo);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
