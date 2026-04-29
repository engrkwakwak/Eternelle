using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.AddEntourageMember;

public sealed record AddEntourageMemberCommand(
    Guid EntourageGroupId,
    string Name,
    string Role,
    string? ImageUrl,
    string? Message,
    string? Note,
    int? Seed,
    int DisplayOrder) : ICommand<Guid>;
