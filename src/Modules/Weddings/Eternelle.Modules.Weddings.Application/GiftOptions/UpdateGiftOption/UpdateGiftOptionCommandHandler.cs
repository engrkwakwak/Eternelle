using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.Shared;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.UpdateGiftOption;

internal sealed class UpdateGiftOptionCommandHandler(
    IGiftOptionRepository giftOptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateGiftOptionCommand>
{
    public async Task<Result> Handle(UpdateGiftOptionCommand command, CancellationToken cancellationToken)
    {
        var giftOptionId = new GiftOptionId(command.GiftOptionId);

        GiftOption? giftOption = await giftOptionRepository.GetAsync(giftOptionId, cancellationToken);

        if (giftOption is null || giftOption.WeddingId.Value != command.WeddingId)
        {
            return Result.Failure(GiftOptionErrors.NotFound(giftOptionId));
        }

        Result<ActivityName> titleResult = ActivityName.Create(command.Title);
        if (titleResult.IsFailure)
        {
            return Result.Failure(titleResult.Error);
        }

        RichDescription? description = null;
        if (command.Description is not null)
        {
            Result<RichDescription> descriptionResult = RichDescription.Create(command.Description);
            if (descriptionResult.IsFailure)
            {
                return Result.Failure(descriptionResult.Error);
            }
            description = descriptionResult.Value;
        }

        WebUrl? linkUrl = null;
        if (command.LinkUrl is not null)
        {
            Result<WebUrl> linkUrlResult = WebUrl.Create(command.LinkUrl);
            if (linkUrlResult.IsFailure)
            {
                return Result.Failure(linkUrlResult.Error);
            }
            linkUrl = linkUrlResult.Value;
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

        ImageUrl? qrImageUrl = null;
        if (command.QrImageUrl is not null)
        {
            Result<ImageUrl> qrImageUrlResult = ImageUrl.Create(command.QrImageUrl);
            if (qrImageUrlResult.IsFailure)
            {
                return Result.Failure(qrImageUrlResult.Error);
            }
            qrImageUrl = qrImageUrlResult.Value;
        }

        AccountHolderName? accountName = null;
        if (command.AccountName is not null)
        {
            Result<AccountHolderName> accountNameResult = AccountHolderName.Create(command.AccountName);
            if (accountNameResult.IsFailure)
            {
                return Result.Failure(accountNameResult.Error);
            }
            accountName = accountNameResult.Value;
        }

        AccountNumber? accountNumber = null;
        if (command.AccountNumber is not null)
        {
            Result<AccountNumber> accountNumberResult = AccountNumber.Create(command.AccountNumber);
            if (accountNumberResult.IsFailure)
            {
                return Result.Failure(accountNumberResult.Error);
            }
            accountNumber = accountNumberResult.Value;
        }

        AccountType? accountType = null;
        if (command.AccountType is not null)
        {
            Result<AccountType> accountTypeResult = AccountType.Create(command.AccountType);
            if (accountTypeResult.IsFailure)
            {
                return Result.Failure(accountTypeResult.Error);
            }
            accountType = accountTypeResult.Value;
        }

        Result result = giftOption.Update(
            titleResult.Value,
            description,
            command.DisplayMode,
            linkUrl,
            imageUrl,
            qrImageUrl,
            accountName,
            accountNumber,
            accountType);

        if (result.IsFailure)
        {
            return result;
        }

        giftOptionRepository.Update(giftOption);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
