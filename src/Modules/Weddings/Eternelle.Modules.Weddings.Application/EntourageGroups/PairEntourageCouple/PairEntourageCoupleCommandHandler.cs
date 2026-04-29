using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.PairEntourageCouple;

internal sealed class PairEntourageCoupleCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<PairEntourageCoupleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(PairEntourageCoupleCommand command, CancellationToken cancellationToken)
    {
        var groupId = new EntourageGroupId(command.EntourageGroupId);

        EntourageGroup? group = await entourageGroupRepository.GetWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure<Guid>(EntourageGroupErrors.NotFound(groupId));
        }

        // Pre-sort IDs to satisfy the DB CHECK constraint (member_a_id < member_b_id).
        // The domain also canonicalises on its own, but we make the intent explicit here.
        (Guid canonicalA, Guid canonicalB) = command.MemberAId.CompareTo(command.MemberBId) < 0
            ? (command.MemberAId, command.MemberBId)
            : (command.MemberBId, command.MemberAId);

        // Append after the highest existing DisplayOrder so removals never cause collisions.
        int displayOrder = group.Couples.Count == 0
            ? 0
            : group.Couples.Max(c => c.DisplayOrder) + 1;

        Result<EntourageCouple> result = group.PairMembers(
            new EntourageMemberId(canonicalA),
            new EntourageMemberId(canonicalB),
            command.Note,
            displayOrder);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id.Value;
    }
}
