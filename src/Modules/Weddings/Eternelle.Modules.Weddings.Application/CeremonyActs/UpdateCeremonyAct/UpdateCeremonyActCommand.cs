using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.UpdateCeremonyAct;

public sealed record UpdateCeremonyActCommand(
    Guid CeremonyActId,
    string Name,
    string? Description,
    string? Icon,
    TimeOnly? ActTime) : ICommand;
