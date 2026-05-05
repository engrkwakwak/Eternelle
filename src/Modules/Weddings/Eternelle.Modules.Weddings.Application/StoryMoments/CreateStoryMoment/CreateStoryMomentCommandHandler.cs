using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.StoryMoments;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.CreateStoryMoment;

internal sealed class CreateStoryMomentCommandHandler(
    IStoryMomentRepository storyMomentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateStoryMomentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStoryMomentCommand command, CancellationToken cancellationToken)
    {
        var storyMoment = StoryMoment.Create(
            new WeddingId(command.WeddingId),
            command.Title,
            command.StoryDate,
            command.Description,
            command.ImageUrl,
            command.DisplayOrder);

        storyMomentRepository.Insert(storyMoment);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return storyMoment.Id.Value;
    }
}
