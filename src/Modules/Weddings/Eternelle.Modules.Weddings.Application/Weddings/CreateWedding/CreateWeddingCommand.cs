using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Weddings.CreateWedding;

public sealed record CreateWeddingCommand(
    Guid TenantId,
    DateOnly WeddingDate,
    string? Hashtag) : ICommand<Guid>;
