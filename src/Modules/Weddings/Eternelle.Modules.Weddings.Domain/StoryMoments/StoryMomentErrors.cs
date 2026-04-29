using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.StoryMoments;

public static class StoryMomentErrors
{
    public static Error NotFound(StoryMomentId id) =>
        Error.NotFound(
            "StoryMoments.NotFound",
            $"The story moment with the identifier {id.Value} was not found");

    public static Error ReorderListMismatch() =>
        Error.Conflict(
            "StoryMoments.ReorderListMismatch",
            "The provided ID list must contain every story moment for this wedding exactly once");
}
