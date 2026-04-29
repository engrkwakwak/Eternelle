using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.CreateEntourageGroup;

internal sealed class CreateEntourageGroupCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateEntourageGroupCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateEntourageGroupCommand command, CancellationToken cancellationToken)
    {
        EntourageGroup group = EntourageGroup.Create(
            new WeddingId(command.WeddingId),
            command.Label,
            command.Subtitle,
            command.GroupType.HasValue ? (EntourageGroupType)command.GroupType.Value : null,
            (EntourageRenderMode)command.RenderAs,
            command.DisplayOrder);

        entourageGroupRepository.Insert(group);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return group.Id.Value;
    }
}
