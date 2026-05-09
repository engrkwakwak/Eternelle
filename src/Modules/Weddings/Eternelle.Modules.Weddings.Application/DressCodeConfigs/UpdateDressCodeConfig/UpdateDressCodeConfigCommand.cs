using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.UpdateDressCodeConfig;

public sealed record UpdateDressCodeConfigCommand(
    Guid DressCodeConfigId,
    string Description) : ICommand;
