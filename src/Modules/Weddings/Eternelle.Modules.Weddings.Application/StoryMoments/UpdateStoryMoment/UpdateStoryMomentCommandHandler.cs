using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.StoryMoments;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.UpdateStoryMoment;

internal sealed class UpdateStoryMomentCommandHandler(
    IStoryMomentRepository storyMomentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateStoryMomentCommand>
{
    public async Task<Result> Handle(UpdateStoryMomentCommand command, CancellationToken cancellationToken)
    {
        var storyMomentId = new StoryMomentId(command.StoryMomentId);

        StoryMoment? storyMoment = await storyMomentRepository.GetAsync(storyMomentId, cancellationToken);

        if (storyMoment is null)
        {
            return Result.Failure(StoryMomentErrors.NotFound(storyMomentId));
        }

        storyMoment.Update(
            command.Title,
            command.StoryDate,
            command.Description,
            command.ImageUrl);

        storyMomentRepository.Update(storyMoment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
