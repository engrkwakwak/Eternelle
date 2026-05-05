using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.GetCeremonyActs;

public sealed record GetCeremonyActsQuery(Guid WeddingId) : IQuery<IReadOnlyList<CeremonyActResponse>>;
