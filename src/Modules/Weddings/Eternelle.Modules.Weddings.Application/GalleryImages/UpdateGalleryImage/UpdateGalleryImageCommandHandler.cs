using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.UpdateGalleryImage;

internal sealed class UpdateGalleryImageCommandHandler(
    IGalleryImageRepository galleryImageRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateGalleryImageCommand>
{
    public async Task<Result> Handle(UpdateGalleryImageCommand command, CancellationToken cancellationToken)
    {
        var imageId = new GalleryImageId(command.GalleryImageId);

        GalleryImage? image = await galleryImageRepository.GetAsync(imageId, cancellationToken);

        if (image is null || image.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(GalleryImageErrors.NotFound(imageId));
        }

        image.Update(
            command.SrcUrl,
            command.AltText,
            command.WidthPx,
            command.HeightPx,
            command.Caption);

        galleryImageRepository.Update(image);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
