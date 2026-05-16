using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageCouple;

public sealed record RemoveEntourageCoupleCommand(Guid WeddingId, Guid EntourageGroupId, Guid EntourageCoupleId) : ICommand;
