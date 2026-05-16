using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GuestPhotos.GetUploadContext;

public sealed record GetUploadContextQuery(Guid UploadToken)
    : IQuery<UploadContextResponse>;
