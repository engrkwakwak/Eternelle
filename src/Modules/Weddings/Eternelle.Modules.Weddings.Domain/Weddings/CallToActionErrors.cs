using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public static class CallToActionErrors
{
    public static readonly Error Empty =
        Error.Problem("CallToAction.Empty", "Call-to-action must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "CallToAction.TooLong",
            $"Call-to-action must not exceed {CallToAction.MaxLength} characters");
}
