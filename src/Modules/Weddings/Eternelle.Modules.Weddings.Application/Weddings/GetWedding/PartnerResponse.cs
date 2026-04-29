namespace Eternelle.Modules.Weddings.Application.Weddings.GetWedding;

public sealed record PartnerResponse(
    Guid Id,
    int PartnerNumber,
    string FirstName,
    string LastName,
    string? Bio,
    string? ImageUrl);
