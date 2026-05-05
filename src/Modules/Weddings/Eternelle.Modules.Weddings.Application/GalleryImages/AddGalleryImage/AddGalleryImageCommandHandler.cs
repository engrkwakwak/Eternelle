using Eternelle.Common.Application.Clock;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.AddGalleryImage;

internal sealed class AddGalleryImageCommandHandler(
    IGalleryImageRepository galleryImageRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<AddGalleryImageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddGalleryImageCommand command, CancellationToken cancellationToken)
    {
        var image = GalleryImage.Create(
            new WeddingId(command.WeddingId),
            command.SrcUrl,
            command.AltText,
            command.WidthPx,
            command.HeightPx,
            command.Caption,
            command.DisplayOrder,
            dateTimeProvider.UtcNow);

        galleryImageRepository.Insert(image);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return image.Id.Value;
    }
}
