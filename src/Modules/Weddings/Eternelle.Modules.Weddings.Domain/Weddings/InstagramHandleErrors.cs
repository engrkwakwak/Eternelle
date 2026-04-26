using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public static class InstagramHandleErrors
{
    public static readonly Error Empty = Error.Problem(
        "InstagramHandle.Empty",
        "Instagram handle must not be empty");

    public static readonly Error TooLong = Error.Problem(
        "InstagramHandle.TooLong",
        $"Instagram handle must not exceed {InstagramHandle.MaxLength} characters");

    public static readonly Error InvalidCharacters = Error.Problem(
        "InstagramHandle.InvalidCharacters",
        "Instagram handle may only contain letters, digits, underscores, and periods");
}
