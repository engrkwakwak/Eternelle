using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.CeremonyActs;

public sealed class CeremonyActCreatedDomainEvent : DomainEvent
{
    public CeremonyActCreatedDomainEvent(CeremonyActId ceremonyActId, WeddingId weddingId)
    {
        CeremonyActId = ceremonyActId;
        WeddingId = weddingId;
    }

    public CeremonyActId CeremonyActId { get; init; }

    public WeddingId WeddingId { get; init; }
}
