using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.ApproveGuestPhoto;

public sealed record ApproveGuestPhotoCommand(Guid WeddingId, Guid GuestPhotoId) : ICommand;
