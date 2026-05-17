using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.CeremonyActs;

public sealed class CeremonyActCreatedDomainEvent(CeremonyActId ceremonyActId, WeddingId weddingId) : DomainEvent
{
    public CeremonyActId CeremonyActId { get; init; } = ceremonyActId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
