using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeColor;

internal sealed class RemoveDressCodeColorCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveDressCodeColorCommand>
{
    public async Task<Result> Handle(
        RemoveDressCodeColorCommand command,
        CancellationToken cancellationToken)
    {
        var colorId = new DressCodeColorId(command.DressCodeColorId);

        DressCodeConfig? config = await dressCodeConfigRepository.GetWithDetailsByColorIdAsync(
            colorId,
            cancellationToken);

        if (config is null || config.Id != new DressCodeConfigId(command.DressCodeConfigId))
        {
            return Result.Failure(DressCodeConfigErrors.ColorNotFound(colorId));
        }

        Result result = config.RemoveColor(colorId);

        if (result.IsFailure)
        {
            return result;
        }

        dressCodeConfigRepository.Update(config);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
