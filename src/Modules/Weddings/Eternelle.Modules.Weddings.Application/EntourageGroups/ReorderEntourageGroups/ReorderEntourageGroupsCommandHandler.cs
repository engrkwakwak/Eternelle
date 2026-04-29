using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.EntourageGroups.ReorderEntourageGroups;

internal sealed class ReorderEntourageGroupsCommandHandler(
    IEntourageGroupRepository entourageGroupRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderEntourageGroupsCommand>
{
    public async Task<Result> Handle(ReorderEntourageGroupsCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<EntourageGroup> groups = await entourageGroupRepository.GetByWeddingIdAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        var groupsById = groups.ToDictionary(g => g.Id.Value);

        HashSet<Guid> providedIds = [.. command.EntourageGroupIds];

        if (command.EntourageGroupIds.Count != providedIds.Count ||
            providedIds.Count != groups.Count ||
            !providedIds.SetEquals(groupsById.Keys))
        {
            return Result.Failure(EntourageGroupErrors.ReorderListMismatch);
        }

        for (int i = 0; i < command.EntourageGroupIds.Count; i++)
        {
            EntourageGroup group = groupsById[command.EntourageGroupIds[i]];
            group.Reorder(i);
            entourageGroupRepository.Update(group);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
