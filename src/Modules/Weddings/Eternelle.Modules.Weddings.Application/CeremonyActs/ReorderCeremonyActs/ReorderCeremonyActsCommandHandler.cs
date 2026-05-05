using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.ReorderCeremonyActs;

internal sealed class ReorderCeremonyActsCommandHandler(
    ICeremonyActRepository ceremonyActRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderCeremonyActsCommand>
{
    public async Task<Result> Handle(
        ReorderCeremonyActsCommand command,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<CeremonyAct> acts = await ceremonyActRepository.GetByWeddingIdAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        var actsById = acts.ToDictionary(a => a.Id.Value);

        HashSet<Guid> providedIds = [.. command.CeremonyActIds];

        if (command.CeremonyActIds.Count != providedIds.Count ||
            providedIds.Count != acts.Count ||
            !providedIds.SetEquals(actsById.Keys))
        {
            return Result.Failure(CeremonyActErrors.ReorderListMismatch);
        }

        for (int i = 0; i < command.CeremonyActIds.Count; i++)
        {
            CeremonyAct act = actsById[command.CeremonyActIds[i]];
            act.Reorder(i);
            ceremonyActRepository.Update(act);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
