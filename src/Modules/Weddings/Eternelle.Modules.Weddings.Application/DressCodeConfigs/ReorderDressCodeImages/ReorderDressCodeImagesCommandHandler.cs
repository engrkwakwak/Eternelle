using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.ReorderDressCodeImages;

internal sealed class ReorderDressCodeImagesCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderDressCodeImagesCommand>
{
    public async Task<Result> Handle(
        ReorderDressCodeImagesCommand command,
        CancellationToken cancellationToken)
    {
        var configId = new DressCodeConfigId(command.DressCodeConfigId);

        DressCodeConfig? config = await dressCodeConfigRepository.GetWithDetailsAsync(configId, cancellationToken);

        if (config is null)
        {
            return Result.Failure(DressCodeConfigErrors.NotFound(configId));
        }

        var imagesById = config.Images.ToDictionary(i => i.Id.Value);

        HashSet<Guid> providedIds = [.. command.DressCodeImageIds];

        if (command.DressCodeImageIds.Count != providedIds.Count ||
            providedIds.Count != config.Images.Count ||
            !providedIds.SetEquals(imagesById.Keys))
        {
            return Result.Failure(DressCodeConfigErrors.ReorderImagesListMismatch);
        }

        for (int i = 0; i < command.DressCodeImageIds.Count; i++)
        {
            Result reorderResult = config.ReorderImage(new DressCodeImageId(command.DressCodeImageIds[i]), i);
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
