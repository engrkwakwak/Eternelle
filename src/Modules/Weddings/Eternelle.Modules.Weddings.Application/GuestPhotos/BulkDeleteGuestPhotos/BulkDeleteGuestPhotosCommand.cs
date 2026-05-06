using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkDeleteGuestPhotos;

public sealed record BulkDeleteGuestPhotosCommand(IReadOnlyList<Guid> GuestPhotoIds) : ICommand;
