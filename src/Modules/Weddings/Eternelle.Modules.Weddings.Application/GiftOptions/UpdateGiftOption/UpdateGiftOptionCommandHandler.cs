using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GiftOptions;

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

        Result result = giftOption.Update(
            command.Title,
            command.Description,
            command.DisplayMode,
            command.LinkUrl,
            command.ImageUrl,
            command.QrImageUrl,
            command.AccountName,
            command.AccountNumber,
            command.AccountType);

        if (result.IsFailure)
        {
            return result;
        }

        giftOptionRepository.Update(giftOption);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
