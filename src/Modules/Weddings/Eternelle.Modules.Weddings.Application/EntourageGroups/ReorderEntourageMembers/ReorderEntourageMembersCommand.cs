using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.ReorderEntourageMembers;

public sealed record ReorderEntourageMembersCommand(
    Guid EntourageGroupId,
    IReadOnlyList<Guid> EntourageMemberIds) : ICommand;
