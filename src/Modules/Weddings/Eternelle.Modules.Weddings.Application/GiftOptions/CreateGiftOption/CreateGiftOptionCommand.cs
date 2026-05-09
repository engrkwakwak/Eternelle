using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.GiftOptions;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.CreateGiftOption;

public sealed record CreateGiftOptionCommand(
    Guid WeddingId,
    string Title,
    string? Description,
    GiftDisplayMode DisplayMode,
    string? LinkUrl,
    string? ImageUrl,
    string? QrImageUrl,
    string? AccountName,
    string? AccountNumber,
    string? AccountType,
    int DisplayOrder) : ICommand<Guid>;
