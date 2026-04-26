using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.StoryMoments;

public sealed class StoryMomentCreatedDomainEvent : DomainEvent
{
    public StoryMomentCreatedDomainEvent(StoryMomentId storyMomentId, WeddingId weddingId)
    {
        StoryMomentId = storyMomentId;
        WeddingId = weddingId;
    }

    public StoryMomentId StoryMomentId { get; init; }

    public WeddingId WeddingId { get; init; }
}
