namespace Eternelle.Modules.Weddings.Domain.StoryMoments;

public readonly record struct StoryMomentId(Guid Value)
{
    public static StoryMomentId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
