namespace Eternelle.Modules.Weddings.Application.EntourageGroups.GetEntourageGroups;

public sealed record EntourageGroupResponse(
    Guid GroupId,
    Guid WeddingId,
    string Label,
    string? Subtitle,
    int? GroupType,
    int RenderAs,
    int DisplayOrder)
{
    public List<EntourageMemberResponse> Members { get; } = [];
    public List<EntourageCoupleResponse> Couples { get; } = [];
}
