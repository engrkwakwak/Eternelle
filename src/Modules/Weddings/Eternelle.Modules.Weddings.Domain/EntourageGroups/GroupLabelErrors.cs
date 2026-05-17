using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public static class GroupLabelErrors
{
    public static readonly Error Empty =
        Error.Problem("GroupLabel.Empty", "Group label must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "GroupLabel.TooLong",
            $"Group label must not exceed {GroupLabel.MaxLength} characters");
}
