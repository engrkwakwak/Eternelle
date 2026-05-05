using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.ReorderEntourageGroups;

public sealed record ReorderEntourageGroupsCommand(
    Guid WeddingId,
    IReadOnlyList<Guid> EntourageGroupIds) : ICommand;
