using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.StoryMoments;

public interface IStoryMomentRepository
{
    Task<StoryMoment?> GetAsync(StoryMomentId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StoryMoment>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(StoryMoment storyMoment);

    void Update(StoryMoment storyMoment);

    void Delete(StoryMoment storyMoment);
}
