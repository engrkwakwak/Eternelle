using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.CeremonyActs;

public static class CeremonyActErrors
{
    public static Error NotFound(CeremonyActId id) =>
        Error.NotFound(
            "CeremonyActs.NotFound",
            $"The ceremony act with the identifier {id.Value} was not found");

    public static readonly Error ReorderListMismatch =
        Error.Problem(
            "CeremonyActs.ReorderListMismatch",
            "The provided ceremony act list does not match the existing acts for this wedding.");
}
