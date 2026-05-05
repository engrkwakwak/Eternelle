using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GalleryImages.GetGalleryImages;

public sealed record GetGalleryImagesQuery(Guid WeddingId) : IQuery<IReadOnlyList<GalleryImageResponse>>;
