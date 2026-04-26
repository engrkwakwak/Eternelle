using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public static class HashtagErrors
{
    public static readonly Error Empty = Error.Problem(
        "Hashtag.Empty",
        "Hashtag must not be empty");

    public static readonly Error TooLong = Error.Problem(
        "Hashtag.TooLong",
        $"Hashtag must not exceed {Hashtag.MaxLength} characters");

    public static readonly Error ContainsSpaces = Error.Problem(
        "Hashtag.ContainsSpaces",
        "Hashtag must not contain spaces");

    public static readonly Error InvalidCharacters = Error.Problem(
        "Hashtag.InvalidCharacters",
        "Hashtag may only contain letters, digits, and underscores");
}
