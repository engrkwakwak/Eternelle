using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Application.GiftOptions.ReorderGiftOptions;

internal sealed class ReorderGiftOptionsCommandHandler(
    IGiftOptionRepository giftOptionRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ReorderGiftOptionsCommand>
{
    public async Task<Result> Handle(ReorderGiftOptionsCommand command, CancellationToken cancellationToken)
    {
        IReadOnlyList<GiftOption> giftOptions = await giftOptionRepository.GetByWeddingIdAsync(
            new WeddingId(command.WeddingId),
            cancellationToken);

        var giftOptionsById = giftOptions.ToDictionary(g => g.Id.Value);

        HashSet<Guid> providedIds = [.. command.GiftOptionIds];

        if (command.GiftOptionIds.Count != providedIds.Count ||
            providedIds.Count != giftOptions.Count ||
            !providedIds.SetEquals(giftOptionsById.Keys))
        {
            return Result.Failure(GiftOptionErrors.ReorderListMismatch);
        }

        for (int i = 0; i < command.GiftOptionIds.Count; i++)
        {
            GiftOption giftOption = giftOptionsById[command.GiftOptionIds[i]];
            giftOption.Reorder(i);
            giftOptionRepository.Update(giftOption);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
