using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.RejectGuestPhoto;

internal sealed class RejectGuestPhotoCommandHandler(
    IGuestPhotoRepository guestPhotoRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<RejectGuestPhotoCommand>
{
    public async Task<Result> Handle(RejectGuestPhotoCommand command, CancellationToken cancellationToken)
    {
        GuestPhoto? photo = await guestPhotoRepository.GetAsync(
            new GuestPhotoId(command.GuestPhotoId),
            cancellationToken);

        if (photo is null || photo.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(GuestPhotoErrors.NotFound);
        }

        Result result = photo.Reject(dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            return result;
        }

        guestPhotoRepository.Update(photo);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
