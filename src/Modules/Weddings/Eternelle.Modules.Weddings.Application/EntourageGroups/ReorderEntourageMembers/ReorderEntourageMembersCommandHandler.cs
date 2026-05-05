using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.ReorderEntourageMembers;

internal sealed class ReorderEntourageMembersCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderEntourageMembersCommand>
{
    public async Task<Result> Handle(ReorderEntourageMembersCommand command, CancellationToken cancellationToken)
    {
        var groupId = new EntourageGroupId(command.EntourageGroupId);

        EntourageGroup? group = await entourageGroupRepository.GetWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure(EntourageGroupErrors.NotFound(groupId));
        }

        var membersById = group.Members.ToDictionary(m => m.Id.Value);

        HashSet<Guid> providedIds = [.. command.EntourageMemberIds];

        if (command.EntourageMemberIds.Count != providedIds.Count ||
            providedIds.Count != group.Members.Count ||
            !providedIds.SetEquals(membersById.Keys))
        {
            return Result.Failure(EntourageGroupErrors.MemberReorderListMismatch);
        }

        for (int i = 0; i < command.EntourageMemberIds.Count; i++)
        {
            group.ReorderMember(new EntourageMemberId(command.EntourageMemberIds[i]), i);
        }

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
