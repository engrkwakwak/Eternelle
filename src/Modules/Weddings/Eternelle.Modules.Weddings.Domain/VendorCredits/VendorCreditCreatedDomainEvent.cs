using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

public sealed class VendorCreditCreatedDomainEvent(VendorCreditId vendorCreditId, WeddingId weddingId) : DomainEvent
{
    public VendorCreditId VendorCreditId { get; init; } = vendorCreditId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
