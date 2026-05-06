using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.CreateCeremonyAct;

public sealed record CreateCeremonyActCommand(
    Guid WeddingId,
    string Name,
    string? Description,
    string? Icon,
    TimeOnly? ActTime) : ICommand<Guid>;
