using Eternelle.Modules.Weddings.Domain.StoryMoments;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.StoryMoments;

internal sealed class StoryMomentRepository(WeddingsDbContext context) : IStoryMomentRepository
{
    public async Task<StoryMoment?> GetAsync(StoryMomentId id, CancellationToken cancellationToken = default)
    {
        return await context.StoryMoments
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<StoryMoment>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.StoryMoments
            .Where(s => s.WeddingId == weddingId)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public void Insert(StoryMoment storyMoment)
    {
        context.StoryMoments.Add(storyMoment);
    }

    public void Update(StoryMoment storyMoment)
    {
        context.StoryMoments.Update(storyMoment);
    }

    public void Delete(StoryMoment storyMoment)
    {
        context.StoryMoments.Remove(storyMoment);
    }
}
