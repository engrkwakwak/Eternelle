using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeColor;

internal sealed class AddDressCodeColorCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddDressCodeColorCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        AddDressCodeColorCommand command,
        CancellationToken cancellationToken)
    {
        var configId = new DressCodeConfigId(command.DressCodeConfigId);

        DressCodeConfig? config = await dressCodeConfigRepository.GetWithDetailsAsync(configId, cancellationToken);

        if (config is null || config.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure<Guid>(DressCodeConfigErrors.NotFound(configId));
        }

        Result<HexColor> hexResult = HexColor.Create(command.ColorHex);

        if (hexResult.IsFailure)
        {
            return Result.Failure<Guid>(hexResult.Error);
        }

        int displayOrder = config.Colors.Count == 0
            ? 0
            : config.Colors.Max(c => c.DisplayOrder) + 1;

        Result<ColorName> colorNameResult = ColorName.Create(command.ColorName);
        if (colorNameResult.IsFailure)
        {
            return Result.Failure<Guid>(colorNameResult.Error);
        }

        DressCodeColor color = config.AddColor(hexResult.Value, colorNameResult.Value, displayOrder);

        dressCodeConfigRepository.Update(config);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return color.Id.Value;
    }
}
