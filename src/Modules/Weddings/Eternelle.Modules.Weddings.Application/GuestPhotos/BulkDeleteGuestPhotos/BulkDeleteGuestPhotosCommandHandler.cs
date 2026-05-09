using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkDeleteGuestPhotos;

internal sealed class BulkDeleteGuestPhotosCommandHandler(
    IGuestPhotoRepository guestPhotoRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<BulkDeleteGuestPhotosCommand>
{
    public async Task<Result> Handle(BulkDeleteGuestPhotosCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<GuestPhotoId> ids = [.. command.GuestPhotoIds.Select(id => new GuestPhotoId(id))];

        IReadOnlyList<GuestPhoto> photos = await guestPhotoRepository.GetManyAsync(ids, cancellationToken);

        if (photos.Count != ids.Count)
        {
            return Result.Failure(GuestPhotoErrors.NotFound);
        }

        foreach (GuestPhoto photo in photos)
        {
            guestPhotoRepository.Delete(photo);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
