using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkApproveGuestPhotos;

internal sealed class BulkApproveGuestPhotosCommandHandler(
    IGuestPhotoRepository guestPhotoRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<BulkApproveGuestPhotosCommand, BulkApproveGuestPhotosResponse>
{
    public async Task<Result<BulkApproveGuestPhotosResponse>> Handle(
        BulkApproveGuestPhotosCommand command,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<GuestPhotoId> ids = [.. command.GuestPhotoIds.Select(id => new GuestPhotoId(id))];

        IReadOnlyList<GuestPhoto> photos = await guestPhotoRepository.GetManyAsync(ids, cancellationToken);

        HashSet<Guid> fetchedIds = [.. photos.Select(p => p.Id.Value)];

        var approvedIds = new List<Guid>();
        var skippedIds = new List<Guid>();

        // IDs not found in DB
        foreach (Guid requestedId in command.GuestPhotoIds)
        {
            if (!fetchedIds.Contains(requestedId))
            {
                skippedIds.Add(requestedId);
            }
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        foreach (GuestPhoto photo in photos)
        {
            Result result = photo.Approve(utcNow);

            if (result.IsSuccess)
            {
                guestPhotoRepository.Update(photo);
                approvedIds.Add(photo.Id.Value);
            }
            else
            {
                skippedIds.Add(photo.Id.Value);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new BulkApproveGuestPhotosResponse(approvedIds, skippedIds));
    }
}
