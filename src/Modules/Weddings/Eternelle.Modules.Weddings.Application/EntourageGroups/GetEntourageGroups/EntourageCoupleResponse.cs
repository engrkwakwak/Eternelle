namespace Eternelle.Modules.Weddings.Application.EntourageGroups.GetEntourageGroups;

public sealed record EntourageCoupleResponse(
    Guid CoupleId,
    Guid GroupId,
    Guid MemberAId,
    Guid MemberBId,
    string? Note,
    int DisplayOrder);
