using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.ReorderDressCodeColors;

public sealed record ReorderDressCodeColorsCommand(
    Guid DressCodeConfigId,
    IReadOnlyList<Guid> DressCodeColorIds) : ICommand;
