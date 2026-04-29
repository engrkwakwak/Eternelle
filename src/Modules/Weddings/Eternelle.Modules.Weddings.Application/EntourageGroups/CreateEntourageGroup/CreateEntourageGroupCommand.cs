using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.CreateEntourageGroup;

public sealed record CreateEntourageGroupCommand(
    Guid WeddingId,
    string Label,
    string? Subtitle,
    int? GroupType,
    int RenderAs,
    int DisplayOrder) : ICommand<Guid>;
