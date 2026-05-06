using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkApproveGuestPhotos;

internal sealed class BulkApproveGuestPhotosCommandHandler(
    IGuestPhotoRepository guestPhotoRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<BulkApproveGuestPhotosCommand>
{
    public async Task<Result> Handle(BulkApproveGuestPhotosCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<GuestPhotoId> ids = [.. command.GuestPhotoIds.Select(id => new GuestPhotoId(id))];

        IReadOnlyList<GuestPhoto> photos = await guestPhotoRepository.GetManyAsync(ids, cancellationToken);

        DateTime utcNow = dateTimeProvider.UtcNow;

        foreach (GuestPhoto photo in photos)
        {
            Result result = photo.Approve(utcNow);

            if (result.IsSuccess)
            {
                guestPhotoRepository.Update(photo);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
