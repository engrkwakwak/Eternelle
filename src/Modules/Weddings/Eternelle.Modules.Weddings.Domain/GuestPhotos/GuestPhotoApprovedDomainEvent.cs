using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public sealed class GuestPhotoApprovedDomainEvent(GuestPhotoId guestPhotoId, WeddingId weddingId, DateTime reviewedAtUtc) : DomainEvent
{
    public GuestPhotoId GuestPhotoId { get; init; } = guestPhotoId;

    public WeddingId WeddingId { get; init; } = weddingId;

    public DateTime ReviewedAtUtc { get; init; } = reviewedAtUtc;
}
