using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.VendorCredits;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.DeleteVendorCredit;

internal sealed class DeleteVendorCreditCommandHandler(
    IVendorCreditRepository vendorCreditRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteVendorCreditCommand>
{
    public async Task<Result> Handle(
        DeleteVendorCreditCommand command,
        CancellationToken cancellationToken)
    {
        var vendorCreditId = new VendorCreditId(command.VendorCreditId);

        VendorCredit? credit = await vendorCreditRepository.GetAsync(vendorCreditId, cancellationToken);

        if (credit is null)
        {
            return Result.Failure(VendorCreditErrors.NotFound(vendorCreditId));
        }

        vendorCreditRepository.Delete(credit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
