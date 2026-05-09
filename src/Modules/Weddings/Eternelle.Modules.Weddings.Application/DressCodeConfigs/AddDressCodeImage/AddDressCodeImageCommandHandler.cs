using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeImage;

internal sealed class AddDressCodeImageCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddDressCodeImageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        AddDressCodeImageCommand command,
        CancellationToken cancellationToken)
    {
        var configId = new DressCodeConfigId(command.DressCodeConfigId);

        DressCodeConfig? config = await dressCodeConfigRepository.GetWithDetailsAsync(configId, cancellationToken);

        if (config is null)
        {
            return Result.Failure<Guid>(DressCodeConfigErrors.NotFound(configId));
        }

        int displayOrder = config.Images.Count == 0
            ? 0
            : config.Images.Max(i => i.DisplayOrder) + 1;

        DressCodeImage image = config.AddImage(command.ImageUrl, displayOrder);

        dressCodeConfigRepository.Update(config);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return image.Id.Value;
    }
}
