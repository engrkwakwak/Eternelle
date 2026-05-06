using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.DeleteVendorCredit;

public sealed record DeleteVendorCreditCommand(Guid VendorCreditId) : ICommand;
