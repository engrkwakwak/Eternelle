using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.DeleteCeremonyAct;

public sealed record DeleteCeremonyActCommand(Guid WeddingId, Guid CeremonyActId) : ICommand;
