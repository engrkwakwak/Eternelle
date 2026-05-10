using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GetGuestPhotos;

public sealed record GetGuestPhotosQuery(
    Guid WeddingId,
    GuestPhotoStatus? Status = null) : IQuery<IReadOnlyList<GuestPhotoResponse>>;
