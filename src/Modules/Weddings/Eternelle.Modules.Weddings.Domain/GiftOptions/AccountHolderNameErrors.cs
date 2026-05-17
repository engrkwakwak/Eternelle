using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public static class AccountHolderNameErrors
{
    public static readonly Error Empty =
        Error.Problem("AccountHolderName.Empty", "Account holder name must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "AccountHolderName.TooLong",
            $"Account holder name must not exceed {AccountHolderName.MaxLength} characters");
}
