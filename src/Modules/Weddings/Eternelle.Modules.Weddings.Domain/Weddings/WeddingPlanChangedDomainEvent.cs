using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingPlanChangedDomainEvent(WeddingId weddingId, WeddingPlan previousPlan, WeddingPlan newPlan) : DomainEvent
{
    public WeddingId WeddingId { get; init; } = weddingId;

    public WeddingPlan PreviousPlan { get; init; } = previousPlan;

    public WeddingPlan NewPlan { get; init; } = newPlan;
}
