using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.CreateGiftOption;

internal sealed class CreateGiftOptionCommandHandler(
    IGiftOptionRepository giftOptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateGiftOptionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGiftOptionCommand command, CancellationToken cancellationToken)
    {
        Result<ActivityName> titleResult = ActivityName.Create(command.Title);
        if (titleResult.IsFailure)
        {
            return Result.Failure<Guid>(titleResult.Error);
        }

        RichDescription? description = null;
        if (command.Description is not null)
        {
            Result<RichDescription> descriptionResult = RichDescription.Create(command.Description);
            if (descriptionResult.IsFailure)
            {
                return Result.Failure<Guid>(descriptionResult.Error);
            }
            description = descriptionResult.Value;
        }

        WebUrl? linkUrl = null;
        if (command.LinkUrl is not null)
        {
            Result<WebUrl> linkUrlResult = WebUrl.Create(command.LinkUrl);
            if (linkUrlResult.IsFailure)
            {
                return Result.Failure<Guid>(linkUrlResult.Error);
            }
            linkUrl = linkUrlResult.Value;
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

        ImageUrl? qrImageUrl = null;
        if (command.QrImageUrl is not null)
        {
            Result<ImageUrl> qrImageUrlResult = ImageUrl.Create(command.QrImageUrl);
            if (qrImageUrlResult.IsFailure)
            {
                return Result.Failure<Guid>(qrImageUrlResult.Error);
            }
            qrImageUrl = qrImageUrlResult.Value;
        }

        AccountHolderName? accountName = null;
        if (command.AccountName is not null)
        {
            Result<AccountHolderName> accountNameResult = AccountHolderName.Create(command.AccountName);
            if (accountNameResult.IsFailure)
            {
                return Result.Failure<Guid>(accountNameResult.Error);
            }
            accountName = accountNameResult.Value;
        }

        AccountNumber? accountNumber = null;
        if (command.AccountNumber is not null)
        {
            Result<AccountNumber> accountNumberResult = AccountNumber.Create(command.AccountNumber);
            if (accountNumberResult.IsFailure)
            {
                return Result.Failure<Guid>(accountNumberResult.Error);
            }
            accountNumber = accountNumberResult.Value;
        }

        AccountType? accountType = null;
        if (command.AccountType is not null)
        {
            Result<AccountType> accountTypeResult = AccountType.Create(command.AccountType);
            if (accountTypeResult.IsFailure)
            {
                return Result.Failure<Guid>(accountTypeResult.Error);
            }
            accountType = accountTypeResult.Value;
        }

        Result<GiftOption> result = GiftOption.Create(
            new WeddingId(command.WeddingId),
            titleResult.Value,
            description,
            command.DisplayMode,
            linkUrl,
            imageUrl,
            qrImageUrl,
            accountName,
            accountNumber,
            accountType,
            command.DisplayOrder);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        giftOptionRepository.Insert(result.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return result.Value.Id.Value;
    }
}
