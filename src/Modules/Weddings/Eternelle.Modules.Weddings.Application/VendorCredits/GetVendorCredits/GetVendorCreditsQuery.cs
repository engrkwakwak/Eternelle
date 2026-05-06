using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.GetVendorCredits;

public sealed record GetVendorCreditsQuery(Guid WeddingId) : IQuery<IReadOnlyList<VendorCreditResponse>>;
