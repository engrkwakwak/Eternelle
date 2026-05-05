using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.UpdateDressCodeConfig;

internal sealed class UpdateDressCodeConfigCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateDressCodeConfigCommand>
{
    public async Task<Result> Handle(
        UpdateDressCodeConfigCommand command,
        CancellationToken cancellationToken)
    {
        var configId = new DressCodeConfigId(command.DressCodeConfigId);

        DressCodeConfig? config = await dressCodeConfigRepository.GetAsync(configId, cancellationToken);

        if (config is null)
        {
            return Result.Failure(DressCodeConfigErrors.NotFound(configId));
        }

        config.UpdateDescription(command.Description);

        dressCodeConfigRepository.Update(config);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
