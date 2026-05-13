using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageMember;

internal sealed class RemoveEntourageMemberCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveEntourageMemberCommand>
{
    public async Task<Result> Handle(RemoveEntourageMemberCommand command, CancellationToken cancellationToken)
    {
        var memberId = new EntourageMemberId(command.EntourageMemberId);

        EntourageGroup? group = await entourageGroupRepository.GetWithMembersByMemberIdAsync(memberId, cancellationToken);

        if (group is null || group.Id != new EntourageGroupId(command.EntourageGroupId))
        {
            return Result.Failure(EntourageGroupErrors.MemberNotFound(memberId));
        }

        Result result = group.RemoveMember(memberId);

        if (result.IsFailure)
        {
            return result;
        }

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
