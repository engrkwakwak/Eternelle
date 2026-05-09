using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.ReorderCeremonyActs;

public sealed record ReorderCeremonyActsCommand(
    Guid WeddingId,
    IReadOnlyList<Guid> CeremonyActIds) : ICommand;
