using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.AddGalleryImage;

internal sealed class AddGalleryImageCommandHandler(
    IGalleryImageRepository galleryImageRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<AddGalleryImageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddGalleryImageCommand command, CancellationToken cancellationToken)
    {
        Result<ImageUrl> srcUrlResult = ImageUrl.Create(command.SrcUrl);
        if (srcUrlResult.IsFailure)
        {
            return Result.Failure<Guid>(srcUrlResult.Error);
        }

        Result<AccessibilityText> altTextResult = AccessibilityText.Create(command.AltText);
        if (altTextResult.IsFailure)
        {
            return Result.Failure<Guid>(altTextResult.Error);
        }

        ImageCaption? caption = null;
        if (command.Caption is not null)
        {
            Result<ImageCaption> captionResult = ImageCaption.Create(command.Caption);
            if (captionResult.IsFailure)
            {
                return Result.Failure<Guid>(captionResult.Error);
            }
            caption = captionResult.Value;
        }

        var image = GalleryImage.Create(
            new WeddingId(command.WeddingId),
            srcUrlResult.Value,
            altTextResult.Value,
            command.WidthPx,
            command.HeightPx,
            caption,
            command.DisplayOrder,
            dateTimeProvider.UtcNow);

        galleryImageRepository.Insert(image);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return image.Id.Value;
    }
}
