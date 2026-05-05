namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

public sealed record PartnerResponse(
    Guid PartnerId,
    int PartnerNumber,
    string FirstName,
    string LastName,
    string? Bio,
    string? ImageUrl);
