using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeImage;

public sealed record AddDressCodeImageCommand(
    Guid WeddingId,
    Guid DressCodeConfigId,
    string ImageUrl) : ICommand<Guid>;
