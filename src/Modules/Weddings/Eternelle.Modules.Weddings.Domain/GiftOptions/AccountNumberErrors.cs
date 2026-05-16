using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public static class AccountNumberErrors
{
    public static readonly Error Empty =
        Error.Problem("AccountNumber.Empty", "Account number must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "AccountNumber.TooLong",
            $"Account number must not exceed {AccountNumber.MaxLength} characters");

    public static readonly Error InvalidFormat =
        Error.Problem(
            "AccountNumber.InvalidFormat",
            "Account number may only contain digits, spaces, and dashes");
}
