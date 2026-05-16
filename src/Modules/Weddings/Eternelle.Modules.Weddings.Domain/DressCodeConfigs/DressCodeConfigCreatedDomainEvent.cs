using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public sealed class DressCodeConfigCreatedDomainEvent(DressCodeConfigId dressCodeConfigId, WeddingId weddingId) : DomainEvent
{
    public DressCodeConfigId DressCodeConfigId { get; init; } = dressCodeConfigId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
