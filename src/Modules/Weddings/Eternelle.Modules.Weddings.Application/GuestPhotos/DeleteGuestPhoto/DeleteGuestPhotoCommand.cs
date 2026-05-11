using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.DeleteGuestPhoto;

public sealed record DeleteGuestPhotoCommand(Guid WeddingId, Guid GuestPhotoId) : ICommand;
