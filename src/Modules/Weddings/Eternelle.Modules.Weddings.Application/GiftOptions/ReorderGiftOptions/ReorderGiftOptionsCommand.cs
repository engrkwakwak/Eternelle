using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.ReorderGiftOptions;

public sealed record ReorderGiftOptionsCommand(
    Guid WeddingId,
    IReadOnlyList<Guid> GiftOptionIds) : ICommand;
