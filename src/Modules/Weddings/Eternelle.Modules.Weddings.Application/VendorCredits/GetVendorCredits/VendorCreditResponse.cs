namespace Eternelle.Modules.Weddings.Application.VendorCredits.GetVendorCredits;

public sealed record VendorCreditResponse(
    Guid Id,
    Guid WeddingId,
    string Name,
    string Role,
    string? WebsiteUrl,
    string? ImageUrl,
    string? InstagramHandle,
    int DisplayOrder);
