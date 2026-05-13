using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageCouple;

internal sealed class RemoveEntourageCoupleCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveEntourageCoupleCommand>
{
    public async Task<Result> Handle(RemoveEntourageCoupleCommand command, CancellationToken cancellationToken)
    {
        var coupleId = new EntourageCoupleId(command.EntourageCoupleId);

        EntourageGroup? group = await entourageGroupRepository.GetWithMembersByCoupleIdAsync(coupleId, cancellationToken);

        if (group is null || group.Id != new EntourageGroupId(command.EntourageGroupId))
        {
            return Result.Failure(EntourageGroupErrors.CoupleNotFound(coupleId));
        }

        Result result = group.RemoveCouple(coupleId);

        if (result.IsFailure)
        {
            return result;
        }

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
