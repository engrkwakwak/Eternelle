using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public static class PersonNameErrors
{
    public static readonly Error Empty =
        Error.Problem("PersonName.Empty", "Name must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "PersonName.TooLong",
            $"Name must not exceed {PersonName.MaxLength} characters");
}
