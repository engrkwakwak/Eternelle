using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GetPublicGuestPhotos;

public sealed record GetPublicGuestPhotosQuery(Guid WeddingId)
    : IQuery<IReadOnlyList<GuestPhotoResponse>>;
