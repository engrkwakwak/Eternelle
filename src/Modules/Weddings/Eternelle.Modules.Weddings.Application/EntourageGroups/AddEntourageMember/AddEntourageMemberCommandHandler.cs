using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.AddEntourageMember;

internal sealed class AddEntourageMemberCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddEntourageMemberCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddEntourageMemberCommand command, CancellationToken cancellationToken)
    {
        var groupId = new EntourageGroupId(command.EntourageGroupId);

        EntourageGroup? group = await entourageGroupRepository.GetWithMembersAsync(groupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure<Guid>(EntourageGroupErrors.NotFound(groupId));
        }

        EntourageMember member = group.AddMember(
            command.Name,
            command.Role,
            command.ImageUrl,
            command.Message,
            command.Note,
            command.Seed,
            command.DisplayOrder);

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return member.Id.Value;
    }
}
