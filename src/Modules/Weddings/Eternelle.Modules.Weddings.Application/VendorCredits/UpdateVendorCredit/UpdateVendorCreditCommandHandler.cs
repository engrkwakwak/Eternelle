using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Shared;
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

        Result<VendorName> nameResult = VendorName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return Result.Failure(nameResult.Error);
        }

        Result<PersonRole> roleResult = PersonRole.Create(command.Role);
        if (roleResult.IsFailure)
        {
            return Result.Failure(roleResult.Error);
        }

        WebUrl? websiteUrl = null;
        if (command.WebsiteUrl is not null)
        {
            Result<WebUrl> websiteUrlResult = WebUrl.Create(command.WebsiteUrl);
            if (websiteUrlResult.IsFailure)
            {
                return Result.Failure(websiteUrlResult.Error);
            }
            websiteUrl = websiteUrlResult.Value;
        }

        ImageUrl? imageUrl = null;
        if (command.ImageUrl is not null)
        {
            Result<ImageUrl> imageUrlResult = ImageUrl.Create(command.ImageUrl);
            if (imageUrlResult.IsFailure)
            {
                return Result.Failure(imageUrlResult.Error);
            }
            imageUrl = imageUrlResult.Value;
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
            nameResult.Value,
            roleResult.Value,
            websiteUrl,
            imageUrl,
            instagramHandle);

        vendorCreditRepository.Update(credit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
