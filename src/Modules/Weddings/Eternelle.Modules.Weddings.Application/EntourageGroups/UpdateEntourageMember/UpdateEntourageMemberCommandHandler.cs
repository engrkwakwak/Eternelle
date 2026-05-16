using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageMember;

internal sealed class UpdateEntourageMemberCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateEntourageMemberCommand>
{
    public async Task<Result> Handle(UpdateEntourageMemberCommand command, CancellationToken cancellationToken)
    {
        var memberId = new EntourageMemberId(command.EntourageMemberId);

        EntourageGroup? group = await entourageGroupRepository.GetWithMembersByMemberIdAsync(memberId, cancellationToken);

        if (group is null || group.WeddingId != new WeddingId(command.WeddingId) || group.Id != new EntourageGroupId(command.EntourageGroupId))
        {
            return Result.Failure(EntourageGroupErrors.MemberNotFound(memberId));
        }

        Result result = group.UpdateMember(
            memberId,
            command.Name,
            command.Role,
            command.ImageUrl,
            command.Message,
            command.Note,
            command.Seed);

        if (result.IsFailure)
        {
            return result;
        }

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
