using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Weddings.RegenerateUploadToken;

public sealed record RegenerateUploadTokenCommand(Guid WeddingId) : ICommand<Guid>;
