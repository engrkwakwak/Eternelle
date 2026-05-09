using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.CreateDressCodeConfig;

public sealed record CreateDressCodeConfigCommand(
    Guid WeddingId,
    string Description) : ICommand<Guid>;
