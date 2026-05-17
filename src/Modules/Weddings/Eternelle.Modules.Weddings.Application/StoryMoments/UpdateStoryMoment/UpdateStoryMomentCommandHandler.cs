using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Shared;
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

        Result<ActivityName> titleResult = ActivityName.Create(command.Title);
        if (titleResult.IsFailure)
        {
            return Result.Failure(titleResult.Error);
        }

        Result<RichDescription> descriptionResult = RichDescription.Create(command.Description);
        if (descriptionResult.IsFailure)
        {
            return Result.Failure(descriptionResult.Error);
        }

        ImageUrl? imageUrl = null;
        if (command.ImageUrl is not null)
        {
            Result<ImageUrl> imageUrlResult = ImageUrl.Create(command.ImageUrl);
            if (imageUrlResult.IsFailure)
            {
                return Result.Failure(imageUrlResult.Error);
            }
            imageUrl = imageUrlResult.Value;
        }

        storyMoment.Update(
            titleResult.Value,
            command.StoryDate,
            descriptionResult.Value,
            imageUrl);

        storyMomentRepository.Update(storyMoment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
