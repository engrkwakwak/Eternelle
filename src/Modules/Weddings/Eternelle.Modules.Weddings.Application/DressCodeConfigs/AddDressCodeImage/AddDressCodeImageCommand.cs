using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeImage;

public sealed record AddDressCodeImageCommand(
    Guid DressCodeConfigId,
    string ImageUrl) : ICommand<Guid>;
