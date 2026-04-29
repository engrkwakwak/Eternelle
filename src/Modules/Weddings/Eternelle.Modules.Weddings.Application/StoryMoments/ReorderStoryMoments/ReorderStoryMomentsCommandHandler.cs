using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.StoryMoments;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.ReorderStoryMoments;

internal sealed class ReorderStoryMomentsCommandHandler(
    IStoryMomentRepository storyMomentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderStoryMomentsCommand>
{
    public async Task<Result> Handle(ReorderStoryMomentsCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<StoryMoment> storyMoments = await storyMomentRepository.GetByWeddingIdAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        var storyMomentsById = storyMoments.ToDictionary(s => s.Id.Value);

        HashSet<Guid> providedIds = [.. command.StoryMomentIds];

        if (command.StoryMomentIds.Count != providedIds.Count ||
            providedIds.Count != storyMoments.Count ||
            !providedIds.SetEquals(storyMomentsById.Keys))
        {
            return Result.Failure(StoryMomentErrors.ReorderListMismatch());
        }

        for (int i = 0; i < command.StoryMomentIds.Count; i++)
        {
            Guid id = command.StoryMomentIds[i];

            if (!storyMomentsById.TryGetValue(id, out StoryMoment? storyMoment))
            {
                return Result.Failure(StoryMomentErrors.NotFound(new StoryMomentId(id)));
            }

            storyMoment.Reorder(i);
            storyMomentRepository.Update(storyMoment);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
