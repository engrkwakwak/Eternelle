using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeColor;

public sealed record AddDressCodeColorCommand(
    Guid DressCodeConfigId,
    string ColorHex,
    string ColorName) : ICommand<Guid>;
