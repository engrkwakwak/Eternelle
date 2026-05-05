using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageCouple;

public sealed record RemoveEntourageCoupleCommand(Guid EntourageCoupleId) : ICommand;
