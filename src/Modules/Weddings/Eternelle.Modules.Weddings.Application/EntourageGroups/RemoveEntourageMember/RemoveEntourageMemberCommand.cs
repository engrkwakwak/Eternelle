using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageMember;

public sealed record RemoveEntourageMemberCommand(Guid EntourageMemberId) : ICommand;
