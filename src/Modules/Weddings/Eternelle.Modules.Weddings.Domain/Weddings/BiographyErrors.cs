using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public static class BiographyErrors
{
    public static readonly Error Empty =
        Error.Problem("Biography.Empty", "Biography must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "Biography.TooLong",
            $"Biography must not exceed {Biography.MaxLength} characters");
}
