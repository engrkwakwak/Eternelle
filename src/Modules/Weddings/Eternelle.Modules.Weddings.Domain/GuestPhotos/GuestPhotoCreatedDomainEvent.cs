using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public sealed class GuestPhotoCreatedDomainEvent : DomainEvent
{
    public GuestPhotoCreatedDomainEvent(GuestPhotoId guestPhotoId, WeddingId weddingId)
    {
        GuestPhotoId = guestPhotoId;
        WeddingId = weddingId;
    }

    public GuestPhotoId GuestPhotoId { get; init; }

    public WeddingId WeddingId { get; init; }
}
