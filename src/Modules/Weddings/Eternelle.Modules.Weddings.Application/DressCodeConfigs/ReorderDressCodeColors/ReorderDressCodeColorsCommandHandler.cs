using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.ReorderDressCodeColors;

internal sealed class ReorderDressCodeColorsCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderDressCodeColorsCommand>
{
    public async Task<Result> Handle(
        ReorderDressCodeColorsCommand command,
        CancellationToken cancellationToken)
    {
        var configId = new DressCodeConfigId(command.DressCodeConfigId);

        DressCodeConfig? config = await dressCodeConfigRepository.GetWithDetailsAsync(configId, cancellationToken);

        if (config is null || config.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(DressCodeConfigErrors.NotFound(configId));
        }

        var colorsById = config.Colors.ToDictionary(c => c.Id.Value);

        HashSet<Guid> providedIds = [.. command.DressCodeColorIds];

        if (command.DressCodeColorIds.Count != providedIds.Count ||
            providedIds.Count != config.Colors.Count ||
            !providedIds.SetEquals(colorsById.Keys))
        {
            return Result.Failure(DressCodeConfigErrors.ReorderColorsListMismatch);
        }

        for (int i = 0; i < command.DressCodeColorIds.Count; i++)
        {
            Result reorderResult = config.ReorderColor(new DressCodeColorId(command.DressCodeColorIds[i]), i);
            if (reorderResult.IsFailure)
            {
                return reorderResult;
            }
        }

        dressCodeConfigRepository.Update(config);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
