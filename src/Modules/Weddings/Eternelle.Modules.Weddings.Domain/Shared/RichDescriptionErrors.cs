using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

internal static class RichDescriptionErrors
{
    public static readonly Error Empty =
        Error.Problem("RichDescription.Empty", "Description must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "RichDescription.TooLong",
            $"Description must not exceed {RichDescription.MaxLength} characters");
}
