using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.RejectGuestPhoto;

public sealed record RejectGuestPhotoCommand(Guid WeddingId, Guid GuestPhotoId) : ICommand;
