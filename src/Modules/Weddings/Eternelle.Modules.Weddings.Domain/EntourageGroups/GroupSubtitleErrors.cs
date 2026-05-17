using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public static class GroupSubtitleErrors
{
    public static readonly Error Empty =
        Error.Problem("GroupSubtitle.Empty", "Group subtitle must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "GroupSubtitle.TooLong",
            $"Group subtitle must not exceed {GroupSubtitle.MaxLength} characters");
}
