namespace Eternelle.Modules.Weddings.Application.GiftOptions.GetGiftOptions;

public sealed record GiftOptionResponse(
    Guid Id,
    Guid WeddingId,
    string Title,
    string? Description,
    string DisplayMode,
    string? LinkUrl,
    string? ImageUrl,
    string? QrImageUrl,
    string? AccountName,
    string? AccountNumber,
    string? AccountType,
    int DisplayOrder);
