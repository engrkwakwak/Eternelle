using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

public static class VendorNameErrors
{
    public static readonly Error Empty =
        Error.Problem("VendorName.Empty", "Vendor name must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "VendorName.TooLong",
            $"Vendor name must not exceed {VendorName.MaxLength} characters");
}
