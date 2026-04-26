using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.CeremonyActs;

public static class CeremonyActErrors
{
    public static Error NotFound(CeremonyActId id) =>
        Error.NotFound(
            "CeremonyActs.NotFound",
            $"The ceremony act with the identifier {id.Value} was not found");
}
