using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Shared;
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

        Result<ImageUrl> srcUrlResult = ImageUrl.Create(command.SrcUrl);
        if (srcUrlResult.IsFailure)
        {
            return Result.Failure(srcUrlResult.Error);
        }

        Result<AccessibilityText> altTextResult = AccessibilityText.Create(command.AltText);
        if (altTextResult.IsFailure)
        {
            return Result.Failure(altTextResult.Error);
        }

        ImageCaption? caption = null;
        if (command.Caption is not null)
        {
            Result<ImageCaption> captionResult = ImageCaption.Create(command.Caption);
            if (captionResult.IsFailure)
            {
                return Result.Failure(captionResult.Error);
            }
            caption = captionResult.Value;
        }

        image.Update(
            srcUrlResult.Value,
            altTextResult.Value,
            command.WidthPx,
            command.HeightPx,
            caption);

        galleryImageRepository.Update(image);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
