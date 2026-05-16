namespace Eternelle.Common.Domain.ValueObjects;

public static class RichDescriptionErrors
{
    public static readonly Error Empty =
        Error.Problem("RichDescription.Empty", "Description must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "RichDescription.TooLong",
            $"Description must not exceed {RichDescription.MaxLength} characters");
}
