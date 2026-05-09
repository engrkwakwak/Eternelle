using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

public static class VendorCreditErrors
{
    public static Error NotFound(VendorCreditId id) =>
        Error.NotFound(
            "VendorCredits.NotFound",
            $"The vendor credit with the identifier {id.Value} was not found");

    public static readonly Error ReorderListMismatch =
        Error.Problem(
            "VendorCredits.ReorderListMismatch",
            "The provided vendor credit list does not match the existing credits for this wedding.");
}
