using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public sealed class DressCodeConfigCreatedDomainEvent : DomainEvent
{
    public DressCodeConfigCreatedDomainEvent(DressCodeConfigId dressCodeConfigId, WeddingId weddingId)
    {
        DressCodeConfigId = dressCodeConfigId;
        WeddingId = weddingId;
    }

    public DressCodeConfigId DressCodeConfigId { get; init; }

    public WeddingId WeddingId { get; init; }
}
