using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Weddings;

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
        IReadOnlyList<Guid> uniqueRawIds = command.GuestPhotoIds.Distinct().ToList();
        IReadOnlyList<GuestPhotoId> ids = [.. uniqueRawIds.Select(id => new GuestPhotoId(id))];

        IReadOnlyList<GuestPhoto> allPhotos = await guestPhotoRepository.GetManyAsync(ids, cancellationToken);

        // Filter to this wedding only — cross-wedding IDs are treated as not found.
        var weddingId = new WeddingId(command.WeddingId);
        IReadOnlyList<GuestPhoto> photos = [.. allPhotos.Where(p => p.WeddingId == weddingId)];

        HashSet<Guid> fetchedIds = [.. photos.Select(p => p.Id.Value)];

        var approvedIds = new List<Guid>();
        var skippedIds = new List<Guid>();

        // IDs not found in DB
        foreach (Guid requestedId in uniqueRawIds)
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
