using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.StoryMoments;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.CreateStoryMoment;

internal sealed class CreateStoryMomentCommandHandler(
    IStoryMomentRepository storyMomentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateStoryMomentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStoryMomentCommand command, CancellationToken cancellationToken)
    {
        Result<ActivityName> titleResult = ActivityName.Create(command.Title);
        if (titleResult.IsFailure)
        {
            return Result.Failure<Guid>(titleResult.Error);
        }

        Result<RichDescription> descriptionResult = RichDescription.Create(command.Description);
        if (descriptionResult.IsFailure)
        {
            return Result.Failure<Guid>(descriptionResult.Error);
        }

        ImageUrl? imageUrl = null;
        if (command.ImageUrl is not null)
        {
            Result<ImageUrl> imageUrlResult = ImageUrl.Create(command.ImageUrl);
            if (imageUrlResult.IsFailure)
            {
                return Result.Failure<Guid>(imageUrlResult.Error);
            }
            imageUrl = imageUrlResult.Value;
        }

        var storyMoment = StoryMoment.Create(
            new WeddingId(command.WeddingId),
            titleResult.Value,
            command.StoryDate,
            descriptionResult.Value,
            imageUrl,
            command.DisplayOrder);

        storyMomentRepository.Insert(storyMoment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return storyMoment.Id.Value;
    }
}
