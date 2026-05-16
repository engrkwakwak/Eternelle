using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.RemoveGalleryImage;

internal sealed class RemoveGalleryImageCommandHandler(
    IGalleryImageRepository galleryImageRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveGalleryImageCommand>
{
    public async Task<Result> Handle(RemoveGalleryImageCommand command, CancellationToken cancellationToken)
    {
        var imageId = new GalleryImageId(command.GalleryImageId);

        GalleryImage? image = await galleryImageRepository.GetAsync(imageId, cancellationToken);

        if (image is null || image.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(GalleryImageErrors.NotFound(imageId));
        }

        galleryImageRepository.Delete(image);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
