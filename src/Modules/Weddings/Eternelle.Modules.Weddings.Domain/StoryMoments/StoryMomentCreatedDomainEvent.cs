using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.StoryMoments;

public sealed class StoryMomentCreatedDomainEvent(StoryMomentId storyMomentId, WeddingId weddingId) : DomainEvent
{
    public StoryMomentId StoryMomentId { get; init; } = storyMomentId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
