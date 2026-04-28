using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdateWeddingDetails;

public sealed record UpdateWeddingDetailsCommand(
    Guid WeddingId,
    DateOnly WeddingDate,
    string? Hashtag) : ICommand;
