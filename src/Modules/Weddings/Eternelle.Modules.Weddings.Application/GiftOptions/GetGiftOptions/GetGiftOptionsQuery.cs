using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.GetGiftOptions;

public sealed record GetGiftOptionsQuery(Guid WeddingId) : IQuery<IReadOnlyList<GiftOptionResponse>>;
