namespace Eternelle.Modules.Weddings.Application.EntourageGroups.GetEntourageGroups;

public sealed record EntourageMemberResponse(
    Guid MemberId,
    Guid GroupId,
    string Name,
    string Role,
    string? ImageUrl,
    string? Message,
    string? Note,
    int? Seed,
    int DisplayOrder);
