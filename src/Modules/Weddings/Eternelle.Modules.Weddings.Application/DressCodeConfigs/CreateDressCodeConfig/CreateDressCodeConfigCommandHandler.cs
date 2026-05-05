using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.CreateDressCodeConfig;

internal sealed class CreateDressCodeConfigCommandHandler(
    IDressCodeConfigRepository dressCodeConfigRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateDressCodeConfigCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateDressCodeConfigCommand command,
        CancellationToken cancellationToken)
    {
        DressCodeConfig config = DressCodeConfig.Create(
            new WeddingId(command.WeddingId),
            command.Description);

        dressCodeConfigRepository.Insert(config);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return config.Id.Value;
    }
}
