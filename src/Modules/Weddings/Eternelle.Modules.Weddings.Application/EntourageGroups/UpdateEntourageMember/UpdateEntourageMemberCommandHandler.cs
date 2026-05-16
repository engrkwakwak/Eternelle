using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Shared;
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

        Result<PersonName> nameResult = PersonName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        Result<PersonRole> roleResult = PersonRole.Create(command.Role);
        if (roleResult.IsFailure)
        {
            return Result.Failure(roleResult.Error);
        }

        ImageUrl? imageUrl = null;
        if (command.ImageUrl is not null)
        {
            Result<ImageUrl> imageUrlResult = ImageUrl.Create(command.ImageUrl);
            if (imageUrlResult.IsFailure)
            {
                return Result.Failure(imageUrlResult.Error);
            }
            imageUrl = imageUrlResult.Value;
        }

        PersonMessage? message = null;
        if (command.Message is not null)
        {
            Result<PersonMessage> messageResult = PersonMessage.Create(command.Message);
            if (messageResult.IsFailure)
            {
                return Result.Failure(messageResult.Error);
            }
            message = messageResult.Value;
        }

        InternalNote? note = null;
        if (command.Note is not null)
        {
            Result<InternalNote> noteResult = InternalNote.Create(command.Note);
            if (noteResult.IsFailure)
            {
                return Result.Failure(noteResult.Error);
            }
            note = noteResult.Value;
        }

        Result result = group.UpdateMember(
            memberId,
            nameResult.Value,
            roleResult.Value,
            imageUrl,
            message,
            note,
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
