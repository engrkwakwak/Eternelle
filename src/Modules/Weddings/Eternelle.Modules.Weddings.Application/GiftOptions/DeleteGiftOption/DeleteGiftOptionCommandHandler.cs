using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GiftOptions;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.DeleteGiftOption;

internal sealed class DeleteGiftOptionCommandHandler(
    IGiftOptionRepository giftOptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteGiftOptionCommand>
{
    public async Task<Result> Handle(DeleteGiftOptionCommand command, CancellationToken cancellationToken)
    {
        var giftOptionId = new GiftOptionId(command.GiftOptionId);

        GiftOption? giftOption = await giftOptionRepository.GetAsync(giftOptionId, cancellationToken);

        if (giftOption is null)
        {
            return Result.Failure(GiftOptionErrors.NotFound(giftOptionId));
        }

        giftOptionRepository.Delete(giftOption);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
