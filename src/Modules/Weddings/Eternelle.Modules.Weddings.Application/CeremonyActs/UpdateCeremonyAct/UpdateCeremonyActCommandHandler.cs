using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

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

        if (act is null || act.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(CeremonyActErrors.NotFound(ceremonyActId));
        }

        Result<ActivityName> nameResult = ActivityName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        RichDescription? description = null;
        if (command.Description is not null)
        {
            Result<RichDescription> descriptionResult = RichDescription.Create(command.Description);
            if (descriptionResult.IsFailure)
            {
                return Result.Failure(descriptionResult.Error);
            }
            description = descriptionResult.Value;
        }

        IconIdentifier? icon = null;
        if (command.Icon is not null)
        {
            Result<IconIdentifier> iconResult = IconIdentifier.Create(command.Icon);
            if (iconResult.IsFailure)
            {
                return Result.Failure(iconResult.Error);
            }
            icon = iconResult.Value;
        }

        act.Update(
            nameResult.Value,
            description,
            icon,
            command.ActTime);

        ceremonyActRepository.Update(act);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
