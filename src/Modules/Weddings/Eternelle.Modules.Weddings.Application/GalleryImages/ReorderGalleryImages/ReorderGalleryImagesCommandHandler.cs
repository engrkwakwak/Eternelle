using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.ReorderGalleryImages;

internal sealed class ReorderGalleryImagesCommandHandler(
    IGalleryImageRepository galleryImageRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderGalleryImagesCommand>
{
    public async Task<Result> Handle(ReorderGalleryImagesCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<GalleryImage> images = await galleryImageRepository.GetByWeddingIdAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        var imagesById = images.ToDictionary(i => i.Id.Value);

        HashSet<Guid> providedIds = [.. command.GalleryImageIds];

        if (command.GalleryImageIds.Count != providedIds.Count ||
            providedIds.Count != images.Count ||
            !providedIds.SetEquals(imagesById.Keys))
        {
            return Result.Failure(GalleryImageErrors.ReorderListMismatch);
        }

        for (int i = 0; i < command.GalleryImageIds.Count; i++)
        {
            GalleryImage image = imagesById[command.GalleryImageIds[i]];
            image.Reorder(i);
            galleryImageRepository.Update(image);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
