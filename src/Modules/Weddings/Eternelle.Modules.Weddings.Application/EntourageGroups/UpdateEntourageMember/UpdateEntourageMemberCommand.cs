using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageMember;

public sealed record UpdateEntourageMemberCommand(
    Guid WeddingId,
    Guid EntourageGroupId,
    Guid EntourageMemberId,
    string Name,
    string Role,
    string? ImageUrl,
    string? Message,
    string? Note,
    int? Seed) : ICommand;
