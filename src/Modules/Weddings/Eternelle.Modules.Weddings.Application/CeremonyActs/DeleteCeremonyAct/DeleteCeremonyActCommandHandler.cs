using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;

namespace Eternelle.Modules.Weddings.Application.CeremonyActs.DeleteCeremonyAct;

internal sealed class DeleteCeremonyActCommandHandler(
    ICeremonyActRepository ceremonyActRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCeremonyActCommand>
{
    public async Task<Result> Handle(
        DeleteCeremonyActCommand command,
        CancellationToken cancellationToken)
    {
        var ceremonyActId = new CeremonyActId(command.CeremonyActId);

        CeremonyAct? act = await ceremonyActRepository.GetAsync(ceremonyActId, cancellationToken);

        if (act is null)
        {
            return Result.Failure(CeremonyActErrors.NotFound(ceremonyActId));
        }

        ceremonyActRepository.Delete(act);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
