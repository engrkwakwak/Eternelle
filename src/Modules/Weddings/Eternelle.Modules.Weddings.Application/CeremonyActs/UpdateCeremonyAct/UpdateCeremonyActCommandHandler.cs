using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.UpdateCeremonyAct;

internal sealed class UpdateCeremonyActCommandHandler(
    ICeremonyActRepository ceremonyActRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCeremonyActCommand>
{
    public async Task<Result> Handle(
        UpdateCeremonyActCommand command,
        CancellationToken cancellationToken)
    {
        var ceremonyActId = new CeremonyActId(command.CeremonyActId);

        CeremonyAct? act = await ceremonyActRepository.GetAsync(ceremonyActId, cancellationToken);

        if (act is null)
        {
            return Result.Failure(CeremonyActErrors.NotFound(ceremonyActId));
        }

        act.Update(
            command.Name,
            command.Description,
            command.Icon,
            command.ActTime);

        ceremonyActRepository.Update(act);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
