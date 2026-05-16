using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public static class PersonFirstNameErrors
{
    public static readonly Error Empty =
        Error.Problem("PersonFirstName.Empty", "First name must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "PersonFirstName.TooLong",
            $"First name must not exceed {PersonFirstName.MaxLength} characters");
}
