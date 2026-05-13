using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeImage;

internal sealed class RemoveDressCodeImageCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveDressCodeImageCommand>
{
    public async Task<Result> Handle(
        RemoveDressCodeImageCommand command,
        CancellationToken cancellationToken)
    {
        var imageId = new DressCodeImageId(command.DressCodeImageId);

        DressCodeConfig? config = await dressCodeConfigRepository.GetWithDetailsByImageIdAsync(
            imageId,
            cancellationToken);

        if (config is null || config.WeddingId != new WeddingId(command.WeddingId) || config.Id != new DressCodeConfigId(command.DressCodeConfigId))
        {
            return Result.Failure(DressCodeConfigErrors.ImageNotFound(imageId));
        }

        Result result = config.RemoveImage(imageId);

        if (result.IsFailure)
        {
            return result;
        }

        dressCodeConfigRepository.Update(config);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
