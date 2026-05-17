using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public static class AccountTypeErrors
{
    public static readonly Error Empty =
        Error.Problem("AccountType.Empty", "Account type must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "AccountType.TooLong",
            $"Account type must not exceed {AccountType.MaxLength} characters");
}
