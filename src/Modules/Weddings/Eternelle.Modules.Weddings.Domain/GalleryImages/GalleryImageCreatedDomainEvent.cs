using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

public sealed class GalleryImageCreatedDomainEvent(GalleryImageId galleryImageId, WeddingId weddingId) : DomainEvent
{
    public GalleryImageId GalleryImageId { get; init; } = galleryImageId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
