using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkDeleteGuestPhotos;

internal sealed class BulkDeleteGuestPhotosCommandHandler(
    IGuestPhotoRepository guestPhotoRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<BulkDeleteGuestPhotosCommand>
{
    public async Task<Result> Handle(BulkDeleteGuestPhotosCommand command, CancellationToken cancellationToken)
    {
        var ids = command.GuestPhotoIds
            .Select(id => new GuestPhotoId(id))
            .Distinct()
            .ToList();

        IReadOnlyList<GuestPhoto> allPhotos = await guestPhotoRepository.GetManyAsync(ids, cancellationToken);

        // Filter to this wedding only — cross-wedding IDs are treated as not found.
        var weddingId = new WeddingId(command.WeddingId);
        IReadOnlyList<GuestPhoto> photos = [.. allPhotos.Where(p => p.WeddingId == weddingId)];

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
