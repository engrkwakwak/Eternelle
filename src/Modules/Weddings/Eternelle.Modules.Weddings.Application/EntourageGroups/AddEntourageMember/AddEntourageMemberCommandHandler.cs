using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.AddEntourageMember;

internal sealed class AddEntourageMemberCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddEntourageMemberCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddEntourageMemberCommand command, CancellationToken cancellationToken)
    {
        var groupId = new EntourageGroupId(command.EntourageGroupId);

        EntourageGroup? group = await entourageGroupRepository.GetWithMembersAsync(groupId, cancellationToken);

        if (group is null || group.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure<Guid>(EntourageGroupErrors.NotFound(groupId));
        }

        Result<PersonName> nameResult = PersonName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return Result.Failure<Guid>(nameResult.Error);
        }

        Result<PersonRole> roleResult = PersonRole.Create(command.Role);
        if (roleResult.IsFailure)
        {
            return Result.Failure<Guid>(roleResult.Error);
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

        PersonMessage? message = null;
        if (command.Message is not null)
        {
            Result<PersonMessage> messageResult = PersonMessage.Create(command.Message);
            if (messageResult.IsFailure)
            {
                return Result.Failure<Guid>(messageResult.Error);
            }
            message = messageResult.Value;
        }

        InternalNote? note = null;
        if (command.Note is not null)
        {
            Result<InternalNote> noteResult = InternalNote.Create(command.Note);
            if (noteResult.IsFailure)
            {
                return Result.Failure<Guid>(noteResult.Error);
            }
            note = noteResult.Value;
        }

        EntourageMember member = group.AddMember(
            nameResult.Value,
            roleResult.Value,
            imageUrl,
            message,
            note,
            command.Seed,
            command.DisplayOrder);

        entourageGroupRepository.Update(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return member.Id.Value;
    }
}
