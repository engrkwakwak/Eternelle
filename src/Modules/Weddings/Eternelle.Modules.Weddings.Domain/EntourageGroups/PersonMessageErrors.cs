using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public static class PersonMessageErrors
{
    public static readonly Error Empty =
        Error.Problem("PersonMessage.Empty", "Message must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "PersonMessage.TooLong",
            $"Message must not exceed {PersonMessage.MaxLength} characters");
}
