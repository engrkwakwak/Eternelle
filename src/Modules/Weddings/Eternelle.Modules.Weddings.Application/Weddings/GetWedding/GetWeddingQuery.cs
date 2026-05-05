using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

public sealed record GetWeddingQuery(Guid WeddingId) : IQuery<WeddingResponse>;
