using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Shared;

public static class PersonRoleErrors
{
    public static readonly Error Empty =
        Error.Problem("PersonRole.Empty", "Role must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "PersonRole.TooLong",
            $"Role must not exceed {PersonRole.MaxLength} characters");
}
