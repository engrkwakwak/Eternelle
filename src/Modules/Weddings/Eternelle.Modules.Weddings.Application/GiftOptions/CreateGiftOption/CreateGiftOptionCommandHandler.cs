using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.CreateGiftOption;

internal sealed class CreateGiftOptionCommandHandler(
    IGiftOptionRepository giftOptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateGiftOptionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateGiftOptionCommand command, CancellationToken cancellationToken)
    {
        Result<GiftOption> result = GiftOption.Create(
            new WeddingId(command.WeddingId),
            command.Title,
            command.Description,
            command.DisplayMode,
            command.LinkUrl,
            command.ImageUrl,
            command.QrImageUrl,
            command.AccountName,
            command.AccountNumber,
            command.AccountType,
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
