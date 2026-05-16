using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.DressCodeConfigs.ReorderDressCodeImages;

public sealed record ReorderDressCodeImagesCommand(
    Guid WeddingId,
    Guid DressCodeConfigId,
    IReadOnlyList<Guid> DressCodeImageIds) : ICommand;
