namespace Eternelle.Modules.Weddings.Application.CeremonyActs.GetCeremonyActs;

public sealed record CeremonyActResponse(
    Guid Id,
    Guid WeddingId,
    string Name,
    string? Description,
    string? Icon,
    TimeOnly? ActTime,
    int DisplayOrder);
