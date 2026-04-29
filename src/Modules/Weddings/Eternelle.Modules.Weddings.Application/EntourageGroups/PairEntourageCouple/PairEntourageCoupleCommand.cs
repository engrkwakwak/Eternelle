using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.PairEntourageCouple;

public sealed record PairEntourageCoupleCommand(
    Guid EntourageGroupId,
    Guid MemberAId,
    Guid MemberBId,
    string? Note) : ICommand<Guid>;
