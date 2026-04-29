using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.StoryMoments;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.DeleteStoryMoment;

internal sealed class DeleteStoryMomentCommandHandler(
    IStoryMomentRepository storyMomentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteStoryMomentCommand>
{
    public async Task<Result> Handle(DeleteStoryMomentCommand command, CancellationToken cancellationToken)
    {
        var storyMomentId = new StoryMomentId(command.StoryMomentId);

        StoryMoment? storyMoment = await storyMomentRepository.GetAsync(storyMomentId, cancellationToken);

        if (storyMoment is null)
        {
            return Result.Failure(StoryMomentErrors.NotFound(storyMomentId));
        }

        storyMomentRepository.Delete(storyMoment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
