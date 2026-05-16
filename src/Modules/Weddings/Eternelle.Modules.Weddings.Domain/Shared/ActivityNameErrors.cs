using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

internal static class ActivityNameErrors
{
    public static readonly Error Empty =
        Error.Problem("ActivityName.Empty", "Name must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "ActivityName.TooLong",
            $"Name must not exceed {ActivityName.MaxLength} characters");
}
