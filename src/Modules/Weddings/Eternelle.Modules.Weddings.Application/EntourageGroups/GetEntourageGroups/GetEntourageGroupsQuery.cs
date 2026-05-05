using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.GetEntourageGroups;

public sealed record GetEntourageGroupsQuery(Guid WeddingId) : IQuery<IReadOnlyList<EntourageGroupResponse>>;
