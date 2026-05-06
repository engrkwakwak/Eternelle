using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.CreateVendorCredit;

public sealed record CreateVendorCreditCommand(
    Guid WeddingId,
    string Name,
    string Role,
    string? WebsiteUrl,
    string? ImageUrl,
    string? InstagramHandle) : ICommand<Guid>;
