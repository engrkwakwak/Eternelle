using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingPlanChangedDomainEvent : DomainEvent
{
    public WeddingPlanChangedDomainEvent(WeddingId weddingId, WeddingPlan previousPlan, WeddingPlan newPlan)
    {
        WeddingId = weddingId;
        PreviousPlan = previousPlan;
        NewPlan = newPlan;
    }

    public WeddingId WeddingId { get; init; }

    public WeddingPlan PreviousPlan { get; init; }

    public WeddingPlan NewPlan { get; init; }
}
