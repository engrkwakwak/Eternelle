using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.EntourageGroups;

public static class InternalNoteErrors
{
    public static readonly Error Empty =
        Error.Problem("InternalNote.Empty", "Note must not be empty");

    public static readonly Error TooLong =
        Error.Problem(
            "InternalNote.TooLong",
            $"Note must not exceed {InternalNote.MaxLength} characters");
}
