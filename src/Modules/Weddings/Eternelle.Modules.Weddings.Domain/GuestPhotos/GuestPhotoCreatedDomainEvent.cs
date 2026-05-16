using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public sealed class GuestPhotoCreatedDomainEvent(GuestPhotoId guestPhotoId, WeddingId weddingId) : DomainEvent
{
    public GuestPhotoId GuestPhotoId { get; init; } = guestPhotoId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
