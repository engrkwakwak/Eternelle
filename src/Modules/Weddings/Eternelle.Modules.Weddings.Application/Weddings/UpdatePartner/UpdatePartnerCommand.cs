using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Weddings.UpdatePartner;

public sealed record UpdatePartnerCommand(
    Guid WeddingId,
    Guid PartnerId,
    string FirstName,
    string LastName,
    string? Bio,
    string? ImageUrl) : ICommand;
