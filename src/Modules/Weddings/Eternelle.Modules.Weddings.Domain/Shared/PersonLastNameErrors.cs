using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public static class PersonLastNameErrors
{
    public static readonly Error Empty =
        Error.Problem("PersonLastName.Empty", "Last name must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "PersonLastName.TooLong",
            $"Last name must not exceed {PersonLastName.MaxLength} characters");
}
