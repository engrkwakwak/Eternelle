using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.DeleteGuestPhoto;

internal sealed class DeleteGuestPhotoCommandHandler(
    IGuestPhotoRepository guestPhotoRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteGuestPhotoCommand>
{
    public async Task<Result> Handle(DeleteGuestPhotoCommand command, CancellationToken cancellationToken)
    {
        GuestPhoto? photo = await guestPhotoRepository.GetAsync(
            new GuestPhotoId(command.GuestPhotoId),
            cancellationToken);

        if (photo is null)
        {
            return Result.Failure(GuestPhotoErrors.NotFound);
        }

        guestPhotoRepository.Delete(photo);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
