using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.ReorderVendorCredits;

public sealed record ReorderVendorCreditsCommand(
    Guid WeddingId,
    IReadOnlyList<Guid> VendorCreditIds) : ICommand;
