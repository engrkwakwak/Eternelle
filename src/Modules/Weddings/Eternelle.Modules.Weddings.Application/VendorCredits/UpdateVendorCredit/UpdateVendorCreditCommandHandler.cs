using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.UpdateVendorCredit;

internal sealed class UpdateVendorCreditCommandHandler(
    IVendorCreditRepository vendorCreditRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateVendorCreditCommand>
{
    public async Task<Result> Handle(
        UpdateVendorCreditCommand command,
        CancellationToken cancellationToken)
    {
        var vendorCreditId = new VendorCreditId(command.VendorCreditId);

        VendorCredit? credit = await vendorCreditRepository.GetAsync(vendorCreditId, cancellationToken);

        if (credit is null || credit.WeddingId != new WeddingId(command.WeddingId))
        {
            return Result.Failure(VendorCreditErrors.NotFound(vendorCreditId));
        }

        InstagramHandle? instagramHandle = null;
        if (!string.IsNullOrWhiteSpace(command.InstagramHandle))
        {
            Result<InstagramHandle> handleResult = InstagramHandle.Create(command.InstagramHandle);
            if (handleResult.IsFailure)
            {
                return Result.Failure(handleResult.Error);
            }
            instagramHandle = handleResult.Value;
        }

        credit.Update(
            command.Name,
            command.Role,
            command.WebsiteUrl,
            command.ImageUrl,
            instagramHandle);

        vendorCreditRepository.Update(credit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
