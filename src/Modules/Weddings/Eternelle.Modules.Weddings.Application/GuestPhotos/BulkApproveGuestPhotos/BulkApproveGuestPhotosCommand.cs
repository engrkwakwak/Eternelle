using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.BulkApproveGuestPhotos;

public sealed record BulkApproveGuestPhotosCommand(Guid WeddingId, IReadOnlyList<Guid> GuestPhotoIds) : ICommand<BulkApproveGuestPhotosResponse>;
