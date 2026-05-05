using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.CreateCeremonyAct;

internal sealed class CreateCeremonyActCommandHandler(
    ICeremonyActRepository ceremonyActRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCeremonyActCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateCeremonyActCommand command,
        CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(command.WeddingId);

        IReadOnlyList<CeremonyAct> existingActs = await ceremonyActRepository.GetByWeddingIdAsync(
            weddingId,
            cancellationToken);

        int displayOrder = existingActs.Count == 0
            ? 0
            : existingActs.Max(a => a.DisplayOrder) + 1;

        var act = CeremonyAct.Create(
            weddingId,
            command.Name,
            command.Description,
            command.Icon,
            command.ActTime,
            displayOrder);

        ceremonyActRepository.Insert(act);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(act.Id.Value);
    }
}
