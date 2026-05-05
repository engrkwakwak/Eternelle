using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.ReorderGalleryImages;

public sealed record ReorderGalleryImagesCommand(
    Guid WeddingId,
    IReadOnlyList<Guid> GalleryImageIds) : ICommand;
