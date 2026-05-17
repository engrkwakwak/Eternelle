using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageGroup;

internal sealed class UpdateEntourageGroupCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateEntourageGroupCommand>
{
    public async Task<Result> Handle(UpdateEntourageGroupCommand command, CancellationToken cancellationToken)
    {
        var groupId = new EntourageGroupId(command.EntourageGroupId);

        EntourageGroup? group = await entourageGroupRepository.GetAsync(groupId, cancellationToken);

        if (group is null || group.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(EntourageGroupErrors.NotFound(groupId));
        }

        Result<GroupLabel> labelResult = GroupLabel.Create(command.Label);
        if (labelResult.IsFailure)
        {
            return Result.Failure(labelResult.Error);
        }

        GroupSubtitle? subtitle = null;
        if (command.Subtitle is not null)
        {
            Result<GroupSubtitle> subtitleResult = GroupSubtitle.Create(command.Subtitle);
            if (subtitleResult.IsFailure)
            {
                return Result.Failure(subtitleResult.Error);
            }
            subtitle = subtitleResult.Value;
        }

        group.UpdateDetails(
            labelResult.Value,
            subtitle,
            command.GroupType.HasValue ? (EntourageGroupType)command.GroupType.Value : group.GroupType,
            (EntourageRenderMode)command.RenderAs);

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
