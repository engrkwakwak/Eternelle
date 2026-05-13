using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageGroup;

public sealed record UpdateEntourageGroupCommand(
    Guid WeddingId,
    Guid EntourageGroupId,
    string Label,
    string? Subtitle,
    int? GroupType,
    int RenderAs) : ICommand;
