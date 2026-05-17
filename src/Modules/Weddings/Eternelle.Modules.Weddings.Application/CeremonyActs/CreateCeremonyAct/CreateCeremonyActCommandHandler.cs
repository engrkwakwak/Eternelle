using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Eternelle.Modules.Weddings.Domain.Shared;
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

        Result<ActivityName> nameResult = ActivityName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return Result.Failure<Guid>(nameResult.Error);
        }

        RichDescription? description = null;
        if (command.Description is not null)
        {
            Result<RichDescription> descriptionResult = RichDescription.Create(command.Description);
            if (descriptionResult.IsFailure)
            {
                return Result.Failure<Guid>(descriptionResult.Error);
            }
            description = descriptionResult.Value;
        }

        IconIdentifier? icon = null;
        if (command.Icon is not null)
        {
            Result<IconIdentifier> iconResult = IconIdentifier.Create(command.Icon);
            if (iconResult.IsFailure)
            {
                return Result.Failure<Guid>(iconResult.Error);
            }
            icon = iconResult.Value;
        }

        var act = CeremonyAct.Create(
            weddingId,
            nameResult.Value,
            description,
            icon,
            command.ActTime,
            displayOrder);

        ceremonyActRepository.Insert(act);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(act.Id.Value);
    }
}
