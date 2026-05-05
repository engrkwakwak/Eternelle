using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.GiftOptions;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.UpdateGiftOption;

public sealed record UpdateGiftOptionCommand(
    Guid GiftOptionId,
    string Title,
    string? Description,
    GiftDisplayMode DisplayMode,
    string? LinkUrl,
    string? ImageUrl,
    string? QrImageUrl,
    string? AccountName,
    string? AccountNumber,
    string? AccountType) : ICommand;
