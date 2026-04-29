using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageGroup;

internal sealed class UpdateEntourageGroupCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateEntourageGroupCommand>
{
    public async Task<Result> Handle(UpdateEntourageGroupCommand command, CancellationToken cancellationToken)
    {
        var groupId = new EntourageGroupId(command.EntourageGroupId);

        EntourageGroup? group = await entourageGroupRepository.GetAsync(groupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure(EntourageGroupErrors.NotFound(groupId));
        }

        group.UpdateDetails(
            command.Label,
            command.Subtitle,
            command.GroupType.HasValue ? (EntourageGroupType)command.GroupType.Value : null,
            (EntourageRenderMode)command.RenderAs);

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
