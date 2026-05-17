using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.VendorCredits.CreateVendorCredit;

internal sealed class CreateVendorCreditCommandHandler(
    IVendorCreditRepository vendorCreditRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVendorCreditCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateVendorCreditCommand command,
        CancellationToken cancellationToken)
    {
        var weddingId = new WeddingId(command.WeddingId);

        IReadOnlyList<VendorCredit> existingCredits = await vendorCreditRepository.GetByWeddingIdAsync(
            weddingId,
            cancellationToken);

        int displayOrder = existingCredits.Count == 0
            ? 0
            : existingCredits.Max(c => c.DisplayOrder) + 1;

        Result<VendorName> nameResult = VendorName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            return Result.Failure<Guid>(nameResult.Error);
        }

        Result<PersonRole> roleResult = PersonRole.Create(command.Role);
        if (roleResult.IsFailure)
        {
            return Result.Failure<Guid>(roleResult.Error);
        }

        WebUrl? websiteUrl = null;
        if (command.WebsiteUrl is not null)
        {
            Result<WebUrl> websiteUrlResult = WebUrl.Create(command.WebsiteUrl);
            if (websiteUrlResult.IsFailure)
            {
                return Result.Failure<Guid>(websiteUrlResult.Error);
            }
            websiteUrl = websiteUrlResult.Value;
        }

        ImageUrl? imageUrl = null;
        if (command.ImageUrl is not null)
        {
            Result<ImageUrl> imageUrlResult = ImageUrl.Create(command.ImageUrl);
            if (imageUrlResult.IsFailure)
            {
                return Result.Failure<Guid>(imageUrlResult.Error);
            }
            imageUrl = imageUrlResult.Value;
        }

        InstagramHandle? instagramHandle = null;
        if (!string.IsNullOrWhiteSpace(command.InstagramHandle))
        {
            Result<InstagramHandle> handleResult = InstagramHandle.Create(command.InstagramHandle);
            if (handleResult.IsFailure)
            {
                return Result.Failure<Guid>(handleResult.Error);
            }
            instagramHandle = handleResult.Value;
        }

        var credit = VendorCredit.Create(
            weddingId,
            nameResult.Value,
            roleResult.Value,
            websiteUrl,
            imageUrl,
            instagramHandle,
            displayOrder);

        vendorCreditRepository.Insert(credit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(credit.Id.Value);
    }
}
