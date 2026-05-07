using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkRejectGuestPhotos;

public sealed record BulkRejectGuestPhotosCommand(IReadOnlyList<Guid> GuestPhotoIds) : ICommand;
