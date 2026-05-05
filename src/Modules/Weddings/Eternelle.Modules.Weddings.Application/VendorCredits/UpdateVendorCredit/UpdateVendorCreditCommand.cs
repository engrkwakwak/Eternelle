using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.UpdateVendorCredit;

public sealed record UpdateVendorCreditCommand(
    Guid VendorCreditId,
    string Name,
    string Role,
    string? WebsiteUrl,
    string? ImageUrl,
    string? InstagramHandle) : ICommand;
