using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public sealed class GuestPhotoApprovedDomainEvent : DomainEvent
{
    public GuestPhotoApprovedDomainEvent(GuestPhotoId guestPhotoId, WeddingId weddingId, DateTime reviewedAtUtc)
    {
        GuestPhotoId = guestPhotoId;
        WeddingId = weddingId;
        ReviewedAtUtc = reviewedAtUtc;
    }

    public GuestPhotoId GuestPhotoId { get; init; }

    public WeddingId WeddingId { get; init; }

    public DateTime ReviewedAtUtc { get; init; }
}
