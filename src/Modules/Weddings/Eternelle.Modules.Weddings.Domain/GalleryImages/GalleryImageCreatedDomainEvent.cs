using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

public sealed class GalleryImageCreatedDomainEvent : DomainEvent
{
    public GalleryImageCreatedDomainEvent(GalleryImageId galleryImageId, WeddingId weddingId)
    {
        GalleryImageId = galleryImageId;
        WeddingId = weddingId;
    }

    public GalleryImageId GalleryImageId { get; init; }

    public WeddingId WeddingId { get; init; }
}
