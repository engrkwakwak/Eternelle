using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.Weddings.AddPartner;

public sealed record AddPartnerCommand(
    Guid WeddingId,
    int PartnerNumber,
    string FirstName,
    string LastName,
    string? Bio,
    string? ImageUrl) : ICommand<Guid>;
