using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.DeleteGiftOption;

public sealed record DeleteGiftOptionCommand(Guid WeddingId, Guid GiftOptionId) : ICommand;
