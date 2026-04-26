using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.StoryMoments;

public static class StoryMomentErrors
{
    public static Error NotFound(StoryMomentId id) =>
        Error.NotFound(
            "StoryMoments.NotFound",
            $"The story moment with the identifier {id.Value} was not found");
}
