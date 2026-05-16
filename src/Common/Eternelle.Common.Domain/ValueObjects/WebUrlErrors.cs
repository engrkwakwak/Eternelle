namespace Eternelle.Common.Domain.ValueObjects;

public static class WebUrlErrors
{
    public static readonly Error Empty =
        Error.Problem("WebUrl.Empty", "Web URL must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "WebUrl.TooLong",
            $"Web URL must not exceed {WebUrl.MaxLength} characters");

    public static readonly Error InvalidFormat =
        Error.Problem(
            "WebUrl.InvalidFormat",
            "Web URL must be a valid absolute http or https URI");
}
