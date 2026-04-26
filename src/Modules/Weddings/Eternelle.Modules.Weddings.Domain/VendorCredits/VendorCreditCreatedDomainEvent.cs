using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

public sealed class VendorCreditCreatedDomainEvent : DomainEvent
{
    public VendorCreditCreatedDomainEvent(VendorCreditId vendorCreditId, WeddingId weddingId)
    {
        VendorCreditId = vendorCreditId;
        WeddingId = weddingId;
    }

    public VendorCreditId VendorCreditId { get; init; }

    public WeddingId WeddingId { get; init; }
}
