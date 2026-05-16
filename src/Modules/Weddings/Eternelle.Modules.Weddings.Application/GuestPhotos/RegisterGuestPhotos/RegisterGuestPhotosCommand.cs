using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.RegisterGuestPhotos;

public sealed record RegisterGuestPhotosCommand(
    Guid UploadToken,
    IReadOnlyList<PhotoRegistration> Photos) : ICommand<IReadOnlyList<Guid>>;
